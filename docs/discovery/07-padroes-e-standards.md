# Padrões e Standards: Convenções de Código e Projeto

**Estado:** 🟢 Definido (Ciclo 3)  
**Última atualização:** 2026-01-26

---

## Filosofia de Padrões

**"Consistência > Perfeição"**

Padrões existem para:
- Facilitar leitura e manutenção
- Reduzir carga cognitiva
- Permitir navegação rápida no código
- Evitar debates infinitos sobre estilo

**Não são sobre:**
- Dogmatismo
- Formatação perfeita
- Seguir frameworks complexos à risca

---

## Backend (.NET 10 + C#)

### 1. Estrutura de Pastas (DDD + CQRS + Monorepo)

```
src/
├── backend/                         # Backend .NET
│   ├── AutorLLM.Api/                # ASP.NET Core Web API
│   │   ├── Controllers/             # Thin controllers (apenas despacham commands/queries)
│   │   ├── Hubs/                    # SignalR hubs
│   │   ├── Middleware/              # Custom middleware
│   │   ├── Program.cs               # Entry point
│   │   └── appsettings.json         # Configuration
│   │
│   ├── AutorLLM.Application/        # Application Layer (CQRS + Use Cases)
│   │   ├── Commands/                # Write operations
│   │   │   ├── Projects/
│   │   │   │   ├── CreateProject/
│   │   │   │   │   ├── CreateProjectCommand.cs
│   │   │   │   │   ├── CreateProjectCommandHandler.cs
│   │   │   │   │   └── CreateProjectCommandValidator.cs
│   │   │   │   ├── UpdateProject/
│   │   │   │   └── DeleteProject/
│   │   │   ├── Characters/
│   │   │   ├── Chapters/
│   │   │   └── LLM/
│   │   │       ├── RewriteText/
│   │   │       └── GenerateOutline/
│   │   │
│   │   ├── Queries/                 # Read operations
│   │   │   ├── Projects/
│   │   │   │   ├── GetProject/
│   │   │   │   │   ├── GetProjectQuery.cs
│   │   │   │   │   └── GetProjectQueryHandler.cs
│   │   │   │   ├── ListProjects/
│   │   │   │   └── GetProjectWithDetails/
│   │   │   ├── Characters/
│   │   │   ├── Chapters/
│   │   │   └── Arcs/
│   │   │
│   │   ├── DTOs/                    # Data transfer objects
│   │   │   ├── ProjectDto.cs
│   │   │   ├── CharacterDto.cs
│   │   │   └── ChapterDto.cs
│   │   │
│   │   ├── Behaviors/               # MediatR pipeline behaviors
│   │   │   ├── ValidationBehavior.cs
│   │   │   ├── LoggingBehavior.cs
│   │   │   └── TransactionBehavior.cs
│   │   │
│   │   └── Interfaces/              # Application interfaces
│   │       └── ILLMOrchestrator.cs
│   │
│   ├── AutorLLM.Domain/             # Domain Layer (DDD)
│   │   ├── Entities/                # Rich domain entities
│   │   │   ├── Project.cs
│   │   │   ├── Character.cs
│   │   │   ├── Chapter.cs
│   │   │   ├── Plot.cs
│   │   │   └── PlotPoint.cs
│   │   │
│   │   ├── ValueObjects/            # Value objects
│   │   │   ├── CharacterRole.cs
│   │   │   ├── PlotType.cs
│   │   │   └── ChapterOrder.cs
│   │   │
│   │   ├── Aggregates/              # Aggregate roots
│   │   │   └── ProjectAggregate/
│   │   │       ├── Project.cs       # Aggregate root
│   │   │       ├── Character.cs     # Entity
│   │   │       └── Chapter.cs       # Entity
│   │   │
│   │   ├── Services/                # Domain services
│   │   │   ├── PlotProgressionService.cs
│   │   │   └── CharacterConsistencyService.cs
│   │   │
│   │   ├── Events/                  # Domain events
│   │   │   ├── ProjectCreatedEvent.cs
│   │   │   ├── ChapterUpdatedEvent.cs
│   │   │   └── CharacterDeletedEvent.cs
│   │   │
│   │   ├── Exceptions/              # Domain exceptions
│   │   │   ├── ProjectNotFoundException.cs
│   │   │   └── InvalidChapterOrderException.cs
│   │   │
│   │   └── Interfaces/              # Repository interfaces (abstractions)
│   │       ├── IProjectRepository.cs
│   │       ├── ICharacterRepository.cs
│   │       └── IUnitOfWork.cs
│   │
│   ├── AutorLLM.Infrastructure/     # Infrastructure Layer
│   │   ├── Data/                    # Data access
│   │   │   ├── Repositories/        # Repository implementations
│   │   │   │   ├── ProjectRepository.cs
│   │   │   │   ├── CharacterRepository.cs
│   │   │   │   └── ChapterRepository.cs
│   │   │   │
│   │   │   ├── Configurations/      # EF Core configurations
│   │   │   │   ├── ProjectConfiguration.cs
│   │   │   │   └── CharacterConfiguration.cs
│   │   │   │
│   │   │   └── AutorLLMDbContext.cs # DbContext
│   │   │
│   │   ├── LLM/                     # LLM integration
│   │   │   ├── LLMOrchestrator.cs
│   │   │   ├── ContextBuilder.cs
│   │   │   └── OllamaClient.cs
│   │   │
│   │   ├── PDF/                     # PDF generation
│   │   │   └── PdfExportService.cs
│   │   │
│   │   └── Supabase/                # Supabase integration
│   │       ├── SupabaseClient.cs
│   │       └── StorageService.cs
│   │
│   ├── AutorLLM.Tests/              # Tests
│   │   ├── Unit/
│   │   │   ├── Domain/
│   │   │   ├── Application/
│   │   │   └── Infrastructure/
│   │   │
│   │   └── Integration/
│   │       ├── Api/
│   │       └── Database/
│   │
│   ├── AutorLLM.sln                 # Solution file
│   └── Directory.Build.props        # Shared build properties
│
└── frontend/                        # Frontend React
    ├── src/
    │   ├── components/              # Componentes reutilizáveis
    │   ├── features/                # Features (Editor, Characters, etc.)
    │   ├── hooks/                   # Custom React hooks
    │   ├── services/                # API clients
    │   ├── stores/                  # Zustand stores
    │   ├── types/                   # TypeScript types
    │   ├── App.tsx
    │   └── main.tsx
    ├── public/
    ├── package.json
    ├── tsconfig.json
    └── vite.config.ts
```

**Rationale:**
- **Clean Architecture** + **DDD** + **CQRS**
- Domain isolado de infra (dependências invertidas)
- Application coordena use cases via Commands/Queries
- Infrastructure implementa persistência e integrações

---

### 2. Naming Conventions

#### Classes e Interfaces
```csharp
// PascalCase para classes
public class ProjectService { }
public class CharacterRepository { }

// Interface com prefixo I
public interface IProjectService { }
public interface ILLMOrchestrator { }

// DTOs com sufixo Dto ou Request/Response
public class CreateProjectDto { }
public class ChapterContentRequest { }
public class GenerateOutlineResponse { }
```

#### Métodos e Propriedades
```csharp
// PascalCase
public async Task<Project> GetProjectAsync(Guid projectId) { }
public string Title { get; set; }

// Async suffix para métodos assíncronos
public async Task<List<Character>> GetCharactersAsync() { }
```

#### Variáveis e Parâmetros
```csharp
// camelCase
var projectId = Guid.NewGuid();
string characterName = "Ana";

// _ prefix para campos privados (opcional, mas recomendado)
private readonly IProjectService _projectService;
```

---

### 3. Async/Await Patterns

**SEMPRE usar async/await para I/O:**
```csharp
// ✅ BOM
public async Task<Project> GetProjectAsync(Guid id)
{
    return await _supabaseClient
        .From<Project>()
        .Where(p => p.Id == id)
        .SingleAsync();
}

// ❌ RUIM (sync I/O)
public Project GetProject(Guid id)
{
    return _supabaseClient
        .From<Project>()
        .Where(p => p.Id == id)
        .Single(); // bloqueia thread
}
```

**ConfigureAwait(false) em bibliotecas:**
```csharp
// Em services/libraries (não em controllers)
var result = await SomeMethodAsync().ConfigureAwait(false);
```

---

### 4. Dependency Injection

**Sempre injetar via construtor:**
```csharp
public class ProjectService : IProjectService
{
    private readonly ISupabaseClient _supabase;
    private readonly ILLMOrchestrator _llm;
    private readonly ILogger<ProjectService> _logger;

    public ProjectService(
        ISupabaseClient supabase,
        ILLMOrchestrator llm,
        ILogger<ProjectService> logger)
    {
        _supabase = supabase;
        _llm = llm;
        _logger = logger;
    }
}
```

**Registrar no Program.cs:**
```csharp
// Scoped para services (um por request)
builder.Services.AddScoped<IProjectService, ProjectService>();

// Singleton para clients (reutilizado)
builder.Services.AddSingleton<ISupabaseClient>(/* config */);

// Transient para leves (novo sempre)
builder.Services.AddTransient<IContextBuilder, ContextBuilder>();
```

---

### 5. Error Handling

**Controllers:**
```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetProject(Guid id)
{
    try
    {
        var project = await _projectService.GetProjectAsync(id);
        if (project == null)
            return NotFound();
        
        return Ok(project);
    }
    catch (SupabaseException ex)
    {
        _logger.LogError(ex, "Supabase error fetching project {ProjectId}", id);
        return StatusCode(500, "Database error");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unexpected error fetching project {ProjectId}", id);
        return StatusCode(500, "Internal server error");
    }
}
```

**Services:**
```csharp
// Deixar exceptions subirem, exceto quando há lógica de retry/fallback
public async Task<Project> GetProjectAsync(Guid id)
{
    // Não catch genérico, deixa controller tratar
    return await _supabase.From<Project>()
        .Where(p => p.Id == id)
        .SingleAsync();
}
```

---

### 6. Logging

**Structured logging com categorias:**
```csharp
// ✅ BOM
_logger.LogInformation(
    "Project {ProjectId} created by user {UserId}",
    project.Id,
    userId
);

// ❌ RUIM (string interpolation)
_logger.LogInformation($"Project {project.Id} created");
```

**Níveis:**
- `Trace`: Muito detalhado (desenvolvimento)
- `Debug`: Detalhes para debugging
- `Information`: Fluxo normal
- `Warning`: Algo anormal mas recuperável
- `Error`: Erro que impede operação
- `Critical`: Sistema instável

---

### 7. DTOs vs Entities

**Nunca expor entidades diretamente na API:**
```csharp
// ❌ RUIM
[HttpGet]
public async Task<List<Project>> GetProjects() { }

// ✅ BOM
[HttpGet]
public async Task<List<ProjectDto>> GetProjects()
{
    var projects = await _service.GetProjectsAsync();
    return projects.Select(p => new ProjectDto
    {
        Id = p.Id,
        Title = p.Title,
        Author = p.Author
        // Não expõe campos internos
    }).ToList();
}
```

---

## Frontend (React + TypeScript)

### 1. Estrutura de Pastas

```
src/
├── components/                 # Componentes React
│   ├── editor/                 # Editor-related
│   │   ├── TextEditor.tsx
│   │   └── EditorToolbar.tsx
│   ├── timeline/               # Timeline visualizer
│   │   └── ArcTimeline.tsx
│   ├── characters/             # Character management
│   │   ├── CharacterList.tsx
│   │   └── CharacterForm.tsx
│   └── common/                 # Reusable components
│       ├── Button.tsx
│       └── Modal.tsx
│
├── hooks/                      # Custom hooks
│   ├── useProject.ts
│   ├── useLLM.ts
│   └── useSupabase.ts
│
├── services/                   # API clients
│   ├── api.ts                  # Axios/Fetch config
│   ├── projectService.ts
│   └── llmService.ts
│
├── store/                      # State management (Zustand)
│   ├── projectStore.ts
│   └── editorStore.ts
│
├── types/                      # TypeScript types
│   ├── project.ts
│   ├── character.ts
│   └── api.ts
│
└── utils/                      # Utility functions
    ├── formatting.ts
    └── validation.ts
```

---

### 2. Component Patterns

**Functional components + hooks:**
```tsx
// ✅ BOM
interface CharacterListProps {
  projectId: string;
  onSelect: (character: Character) => void;
}

export const CharacterList: React.FC<CharacterListProps> = ({ 
  projectId, 
  onSelect 
}) => {
  const { characters, loading } = useCharacters(projectId);

  if (loading) return <Spinner />;

  return (
    <ul>
      {characters.map(char => (
        <li key={char.id} onClick={() => onSelect(char)}>
          {char.name}
        </li>
      ))}
    </ul>
  );
};
```

**Evitar class components:**
```tsx
// ❌ EVITAR (legacy)
class CharacterList extends React.Component { }
```

---

### 3. Naming Conventions

```tsx
// Components: PascalCase
export const TextEditor = () => { };
export const ArcTimeline = () => { };

// Files: PascalCase matching component name
// TextEditor.tsx, ArcTimeline.tsx

// Hooks: camelCase com prefixo use
export const useProject = () => { };
export const useLLMStream = () => { };

// Types/Interfaces: PascalCase
interface Character { }
type ProjectStatus = 'draft' | 'published';

// Constants: UPPER_SNAKE_CASE
export const MAX_CHAPTER_LENGTH = 10000;
export const API_BASE_URL = 'http://localhost:5000';
```

---

### 4. State Management (Zustand)

**Criar stores focados:**
```typescript
// store/projectStore.ts
interface ProjectStore {
  currentProject: Project | null;
  setCurrentProject: (project: Project) => void;
  updateProject: (updates: Partial<Project>) => void;
}

export const useProjectStore = create<ProjectStore>((set) => ({
  currentProject: null,
  setCurrentProject: (project) => set({ currentProject: project }),
  updateProject: (updates) => set((state) => ({
    currentProject: state.currentProject 
      ? { ...state.currentProject, ...updates }
      : null
  }))
}));
```

**Usar nos components:**
```tsx
const Editor = () => {
  const currentProject = useProjectStore(state => state.currentProject);
  const updateProject = useProjectStore(state => state.updateProject);

  // ...
};
```

---

### 5. API Calls

**Centralizar em services:**
```typescript
// services/projectService.ts
import axios from 'axios';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  headers: { 'Content-Type': 'application/json' }
});

export const projectService = {
  getAll: async (): Promise<Project[]> => {
    const { data } = await api.get('/projects');
    return data;
  },

  create: async (project: CreateProjectDto): Promise<Project> => {
    const { data } = await api.post('/projects', project);
    return data;
  },

  // ...
};
```

**Usar em hooks:**
```typescript
// hooks/useProjects.ts
export const useProjects = () => {
  const [projects, setProjects] = useState<Project[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    projectService.getAll()
      .then(setProjects)
      .finally(() => setLoading(false));
  }, []);

  return { projects, loading };
};
```

---

### 6. TypeScript

**Sempre tipar explicitamente interfaces públicas:**
```typescript
// ✅ BOM
interface Character {
  id: string;
  name: string;
  role: 'protagonist' | 'antagonist' | 'supporting';
}

const updateCharacter = (char: Character): Promise<void> => { };

// ❌ EVITAR (any)
const updateCharacter = (char: any) => { };
```

**Usar tipos utilitários:**
```typescript
// Partial para updates
type CharacterUpdate = Partial<Character>;

// Pick para selecionar campos
type CharacterPreview = Pick<Character, 'id' | 'name'>;

// Omit para excluir campos
type NewCharacter = Omit<Character, 'id'>;
```

---

## Git & Commits

### 1. Commit Messages

**Padrão Conventional Commits:**
```
<type>(<scope>): <subject>

<body>
```

**Types:**
- `feat`: Nova feature
- `fix`: Bug fix
- `docs`: Documentação
- `refactor`: Refatoração (sem mudar comportamento)
- `test`: Adicionar/modificar testes
- `chore`: Tarefas de build, deps, etc.

**Exemplos:**
```
feat(editor): add LLM rewrite command
fix(api): handle null project in GetProject
docs(readme): update setup instructions
refactor(services): extract context builder logic
```

---

### 2. Branches

**Padrão:**
- `main`: produção (sempre estável)
- `develop`: desenvolvimento ativo
- `feature/nome-da-feature`: novas features
- `fix/nome-do-bug`: correções

**Workflow:**
```bash
# Criar feature branch
git checkout -b feature/arc-visualization

# Commits frequentes
git commit -m "feat(timeline): add basic arc chart"

# Merge via PR (Pull Request)
```

---

## Testes

### 1. Backend (xUnit + Moq)

```csharp
public class ProjectServiceTests
{
    [Fact]
    public async Task GetProjectAsync_ExistingId_ReturnsProject()
    {
        // Arrange
        var mockRepo = new Mock<IProjectRepository>();
        mockRepo.Setup(r => r.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Project { Id = Guid.NewGuid() });
        
        var service = new ProjectService(mockRepo.Object);

        // Act
        var result = await service.GetProjectAsync(Guid.NewGuid());

        // Assert
        Assert.NotNull(result);
    }
}
```

---

### 2. Frontend (Vitest + Testing Library)

```typescript
import { render, screen } from '@testing-library/react';
import { CharacterList } from './CharacterList';

describe('CharacterList', () => {
  it('renders characters', () => {
    const characters = [
      { id: '1', name: 'Ana', role: 'protagonist' }
    ];

    render(<CharacterList characters={characters} />);

    expect(screen.getByText('Ana')).toBeInTheDocument();
  });
});
```

---

## Segurança

### 1. Variáveis de Ambiente

**Backend (appsettings.json):**
```json
{
  "Supabase": {
    "Url": "https://xxxxx.supabase.co",
    "Key": "USE_ENV_VAR"
  },
  "Ollama": {
    "Endpoint": "http://localhost:11434"
  }
}
```

**Nunca commitar secrets:**
```bash
# .gitignore
appsettings.Development.json
.env
.env.local
```

---

### 2. Validação de Input

**Backend:**
```csharp
[HttpPost]
public async Task<IActionResult> CreateProject(CreateProjectDto dto)
{
    if (string.IsNullOrWhiteSpace(dto.Title))
        return BadRequest("Title is required");

    if (dto.Title.Length > 200)
        return BadRequest("Title too long");

    // ...
}
```

**Frontend:**
```typescript
const validateProject = (project: CreateProjectDto): string[] => {
  const errors: string[] = [];

  if (!project.title?.trim()) {
    errors.push('Title is required');
  }

  if (project.title && project.title.length > 200) {
    errors.push('Title must be under 200 characters');
  }

  return errors;
};
```

---

## Próximos Passos

- [ ] Criar template de projeto (.NET solution + React app)
- [ ] Setup CI/CD básico (GitHub Actions)
- [ ] Configurar linters (ESLint + StyleCop)
- [ ] Criar ADRs para decisões arquiteturais importantes
