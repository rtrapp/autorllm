# Padrões DDD + CQRS: Implementação e Convenções

**Estado:** 🟢 Definido  
**Última atualização:** 2026-01-26

---

## 1. CQRS Pattern

### O que é CQRS?

**Command Query Responsibility Segregation** = Separação de Responsabilidades entre Comandos e Consultas.

**Comandos (Write):**
- Alteram estado do sistema
- Não retornam dados (apenas confirmação)
- Exemplos: CreateProject, UpdateChapter, DeleteCharacter

**Queries (Read):**
- Não alteram estado
- Retornam dados
- Otimizadas para leitura
- Exemplos: GetProject, ListCharacters, GetChapterContent

---

## 2. Estrutura de Commands

### Command (Mensagem)

```csharp
// src/backend/AutorLLM.Application/Commands/Projects/CreateProject/CreateProjectCommand.cs

public record CreateProjectCommand : IRequest<CreateProjectResult>
{
    public string Title { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public string Synopsis { get; init; } = string.Empty;
}
```

**Convenções:**
- `record` (imutável)
- Nome: `{Verbo}{Entidade}Command`
- Implementa `IRequest<TResponse>` (MediatR)
- Apenas dados necessários para a operação

---

### Command Handler

```csharp
// src/backend/AutorLLM.Application/Commands/Projects/CreateProject/CreateProjectCommandHandler.cs

public class CreateProjectCommandHandler 
    : IRequestHandler<CreateProjectCommand, CreateProjectResult>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateProjectCommandHandler> _logger;

    public CreateProjectCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateProjectCommandHandler> logger)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CreateProjectResult> Handle(
        CreateProjectCommand command, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating project: {Title} by {Author}", 
            command.Title, 
            command.Author
        );

        // 1. Criar entidade de domínio
        var project = Project.Create(
            command.Title, 
            command.Author, 
            command.Synopsis
        );

        // 2. Salvar via repository
        await _projectRepository.AddAsync(project, cancellationToken);

        // 3. Commit (Unit of Work)
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Project created with ID: {ProjectId}", project.Id);

        // 4. Retornar resultado
        return new CreateProjectResult 
        { 
            ProjectId = project.Id,
            Success = true 
        };
    }
}
```

**Convenções:**
- Nome: `{Command}Handler`
- Um handler por command
- Lógica de orquestração (coordena domínio + infra)
- Logging estruturado
- Transaction via Unit of Work

---

### Command Validator (FluentValidation)

```csharp
// src/backend/AutorLLM.Application/Commands/Projects/CreateProject/CreateProjectCommandValidator.cs

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must be under 200 characters");

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("Author is required")
            .MaximumLength(100).WithMessage("Author must be under 100 characters");

        RuleFor(x => x.Synopsis)
            .MaximumLength(2000).WithMessage("Synopsis must be under 2000 characters");
    }
}
```

**Convenções:**
- Nome: `{Command}Validator`
- FluentValidation para validação de input
- Executado automaticamente via `ValidationBehavior` (pipeline MediatR)

---

## 3. Estrutura de Queries

### Query (Mensagem)

```csharp
// src/backend/AutorLLM.Application/Queries/Projects/GetProject/GetProjectQuery.cs

public record GetProjectQuery : IRequest<ProjectDto>
{
    public Guid ProjectId { get; init; }
}
```

**Convenções:**
- `record` (imutável)
- Nome: `Get{Entidade}Query` ou `List{Entidade}Query`
- Implementa `IRequest<TResponse>`
- Retorna DTO, nunca entidade de domínio

---

### Query Handler

```csharp
// src/backend/AutorLLM.Application/Queries/Projects/GetProject/GetProjectQueryHandler.cs

public class GetProjectQueryHandler 
    : IRequestHandler<GetProjectQuery, ProjectDto>
{
    private readonly ISupabaseClient _supabase;
    private readonly ILogger<GetProjectQueryHandler> _logger;

    public GetProjectQueryHandler(
        ISupabaseClient supabase,
        ILogger<GetProjectQueryHandler> logger)
    {
        _supabase = supabase;
        _logger = logger;
    }

    public async Task<ProjectDto> Handle(
        GetProjectQuery query, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching project: {ProjectId}", query.ProjectId);

        // Query diretamente do banco (pode pular domínio para performance)
        var project = await _supabase
            .From<ProjectDto>()
            .Where(p => p.Id == query.ProjectId)
            .SingleAsync(cancellationToken);

        if (project == null)
        {
            throw new ProjectNotFoundException(query.ProjectId);
        }

        return project;
    }
}
```

**Convenções:**
- Nome: `{Query}Handler`
- Pode bypassar domínio (ler direto do DB)
- Retorna DTOs otimizados para UI
- Sem lógica de negócio complexa

---

## 4. Domain-Driven Design (DDD)

### Entities (Entidades Ricas)

```csharp
// src/backend/AutorLLM.Domain/Entities/Project.cs

public class Project
{
    // Construtor privado (factory pattern)
    private Project() { }

    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Author { get; private set; } = string.Empty;
    public string Synopsis { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Navegação (aggregate)
    private readonly List<Character> _characters = new();
    public IReadOnlyCollection<Character> Characters => _characters.AsReadOnly();

    private readonly List<Chapter> _chapters = new();
    public IReadOnlyCollection<Chapter> Chapters => _chapters.AsReadOnly();

    // Factory method (encapsula criação)
    public static Project Create(string title, string author, string synopsis)
    {
        // Validações de domínio
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        if (title.Length > 200)
            throw new ArgumentException("Title too long", nameof(title));

        var project = new Project
        {
            Id = Guid.NewGuid(),
            Title = title.Trim(),
            Author = author.Trim(),
            Synopsis = synopsis.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Domain event (opcional)
        project.AddDomainEvent(new ProjectCreatedEvent(project.Id));

        return project;
    }

    // Métodos de comportamento (não setters públicos!)
    public void UpdateTitle(string newTitle)
    {
        if (string.IsNullOrWhiteSpace(newTitle))
            throw new ArgumentException("Title cannot be empty");

        Title = newTitle.Trim();
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ProjectUpdatedEvent(Id));
    }

    public void AddCharacter(Character character)
    {
        if (_characters.Any(c => c.Name == character.Name))
            throw new InvalidOperationException(
                $"Character with name '{character.Name}' already exists"
            );

        _characters.Add(character);
        UpdatedAt = DateTime.UtcNow;
    }

    public void ReorderChapters(List<Guid> chapterIds)
    {
        // Lógica de domínio para reordenar capítulos
        for (int i = 0; i < chapterIds.Count; i++)
        {
            var chapter = _chapters.FirstOrDefault(c => c.Id == chapterIds[i]);
            if (chapter != null)
            {
                chapter.UpdateOrder(i + 1);
            }
        }

        UpdatedAt = DateTime.UtcNow;
    }

    // Domain events (opcional, mas recomendado)
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
```

**Convenções DDD:**
- Setters privados (encapsulamento)
- Factory methods para criação (`Create()`)
- Métodos de comportamento (não expor `set`)
- Validações de domínio na entidade
- Domain events para side effects

---

### Value Objects

```csharp
// src/backend/AutorLLM.Domain/ValueObjects/CharacterRole.cs

public record CharacterRole
{
    public string Value { get; init; }

    private CharacterRole(string value) => Value = value;

    public static CharacterRole Protagonist => new("Protagonist");
    public static CharacterRole Antagonist => new("Antagonist");
    public static CharacterRole Supporting => new("Supporting");
    public static CharacterRole Minor => new("Minor");

    public static CharacterRole FromString(string value)
    {
        return value switch
        {
            "Protagonist" => Protagonist,
            "Antagonist" => Antagonist,
            "Supporting" => Supporting,
            "Minor" => Minor,
            _ => throw new ArgumentException($"Invalid role: {value}")
        };
    }

    public override string ToString() => Value;
}
```

**Convenções:**
- `record` (imutável, equality by value)
- Sem ID
- Encapsula lógica de validação
- Static factory methods

---

### Domain Services

```csharp
// src/backend/AutorLLM.Domain/Services/PlotProgressionService.cs

public class PlotProgressionService
{
    public bool ValidatePlotProgression(Plot plot, List<Chapter> chapters)
    {
        // Lógica de domínio que envolve múltiplas entidades
        var plotPoints = plot.PlotPoints.OrderBy(pp => pp.Chapter.Order).ToList();

        // Validar se plot tem pelo menos início, meio e fim
        if (plotPoints.Count < 3)
            return false;

        // Validar se intensidade cresce até clímax
        var maxIntensity = plotPoints.Max(pp => pp.Intensity);
        var maxIntensityChapter = plotPoints.First(pp => pp.Intensity == maxIntensity);

        // Clímax deve estar na segunda metade da história
        var totalChapters = chapters.Count;
        if (maxIntensityChapter.Chapter.Order < totalChapters / 2)
            return false;

        return true;
    }
}
```

**Quando usar Domain Service:**
- Lógica de domínio que envolve múltiplas entidades
- Não pertence a nenhuma entidade específica
- Sem estado (stateless)

---

## 5. MediatR Pipeline Behaviors

### Validation Behavior

```csharp
// src/backend/AutorLLM.Application/Behaviors/ValidationBehavior.cs

public class ValidationBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))
            );

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }
        }

        return await next();
    }
}
```

### Logging Behavior

```csharp
// src/backend/AutorLLM.Application/Behaviors/LoggingBehavior.cs

public class LoggingBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation(
            "Handling {RequestName}", 
            requestName
        );

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next();

            stopwatch.Stop();

            _logger.LogInformation(
                "Handled {RequestName} in {ElapsedMilliseconds}ms",
                requestName,
                stopwatch.ElapsedMilliseconds
            );

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "Error handling {RequestName} after {ElapsedMilliseconds}ms",
                requestName,
                stopwatch.ElapsedMilliseconds
            );

            throw;
        }
    }
}
```

---

## 6. Controllers (Thin Controllers)

```csharp
// src/backend/AutorLLM.Api/Controllers/ProjectsController.cs

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject(
        [FromBody] CreateProjectCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(
            nameof(GetProject), 
            new { id = result.ProjectId }, 
            result
        );
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProject(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetProjectQuery { ProjectId = id };
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(
        Guid id,
        [FromBody] UpdateProjectCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.ProjectId)
            return BadRequest("ID mismatch");

        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteProjectCommand { ProjectId = id };
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
```

**Convenções:**
- Controllers só despacham commands/queries via MediatR
- Não contêm lógica de negócio
- Retornam status codes apropriados

---

## 7. Dependency Injection Setup

```csharp
// Program.cs

var builder = WebApplication.CreateBuilder(args);

// MediatR + Behaviors
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(CreateProjectCommand).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
});

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(CreateProjectCommandValidator).Assembly);

// Repositories
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Domain Services
builder.Services.AddScoped<PlotProgressionService>();

// Infrastructure
builder.Services.AddScoped<ILLMOrchestrator, LLMOrchestrator>();
builder.Services.AddSingleton<ISupabaseClient>(/* config */);

var app = builder.Build();
```

---

## 8. Resumo de Convenções

### Commands:
- ✅ `record` imutável
- ✅ Nome: `{Verbo}{Entidade}Command`
- ✅ Handler dedicado
- ✅ Validator (FluentValidation)
- ✅ Retorna resultado (não entidade)

### Queries:
- ✅ `record` imutável
- ✅ Nome: `Get/List{Entidade}Query`
- ✅ Handler dedicado
- ✅ Retorna DTO otimizado
- ✅ Pode bypassar domínio

### Domain:
- ✅ Entidades ricas (comportamento)
- ✅ Setters privados
- ✅ Factory methods
- ✅ Validações internas
- ✅ Domain events

### Controllers:
- ✅ Thin (só despacham)
- ✅ MediatR para orquestração
- ✅ Sem lógica de negócio

---

## Próximos Passos

- [ ] Implementar base classes (Command, Query, Entity)
- [ ] Configurar MediatR + Behaviors
- [ ] Criar primeiro Use Case completo (CreateProject)
- [ ] Testar pipeline de validação
