# Integração Microsoft Agent Framework - Guia de Implementação

**Última atualização:** 2026-01-27  
**Status:** 🟡 Em Planejamento

---

## Visão Geral

Este documento define como implementar a integração com **Microsoft Agent Framework**, **Semantic Kernel** e **Microsoft.Extensions.AI** nas User Stories US072-US075.

---

## Stack Obrigatória

```yaml
Pacotes NuGet Obrigatórios:
  - Microsoft.Extensions.AI: 10.0.x (abstração IChatClient)
  - Microsoft.Extensions.AI.Ollama: 10.0.x (connector Ollama)
  - Microsoft.Agents.AI: prerelease (AIAgent)
  - Microsoft.Extensions.DependencyInjection: 10.0.x
  - Microsoft.Extensions.Logging: 10.0.x
```

**Referência:** https://learn.microsoft.com/en-us/agent-framework/tutorials/quick-start

---

## Arquitetura de Integração

```
┌─────────────────────────────────────────────────────┐
│                   LLMHub (SignalR)                   │
│                                                      │
│  - RequestRewrite(chapterId, text, command)         │
│  - Usa AIAgent para processar requisições           │
└─────────────────────────────────────────────────────┘
                       ↓
┌─────────────────────────────────────────────────────┐
│            AIAgent (Agent Framework)                 │
│                                                      │
│  - RunStreamingAsync(message)                       │
│  - Retorna IAsyncEnumerable<AgentResponseUpdate>    │
└─────────────────────────────────────────────────────┘
                       ↓
┌─────────────────────────────────────────────────────┐
│   IChatClient (Microsoft.Extensions.AI.Ollama)      │
│                                                      │
│  - Abstração para qualquer provedor LLM             │
│  - Ollama connector implementa IChatClient          │
│  - Streaming via IAsyncEnumerable<StreamingChatCompletionUpdate> │
└─────────────────────────────────────────────────────┘
                       ↓
┌─────────────────────────────────────────────────────┐
│          Ollama (http://localhost:11434)             │
│                                                      │
│  - Modelo: gpt-oss:20b (ou configurável)            │
│  - API REST com streaming                           │
└─────────────────────────────────────────────────────┘
```

---

## US072: Conectar Backend ao Ollama

### Objetivo
Configurar Microsoft Agent Framework com Ollama usando IChatClient e criar AIAgent.

### Implementação

#### 1. Adicionar Pacotes NuGet
```bash
cd src/backend/AutorLLM.Infrastructure
dotnet add package Microsoft.Extensions.AI --version 10.0.x
dotnet add package Microsoft.Extensions.AI.Ollama --version 10.0.x
dotnet add package Microsoft.Agents.AI --prerelease
```

#### 2. Criar Configuração (appsettings.json)
```json
{
  "AgentFramework": {
    "Ollama": {
      "Endpoint": "http://localhost:11434",
      "Model": "gpt-oss:20b"
    }
  }
}
```

#### 3. Criar Options Class
```csharp
// AutorLLM.Infrastructure/Configuration/AgentFrameworkOptions.cs
namespace AutorLLM.Infrastructure.Configuration;

public class AgentFrameworkOptions
{
    public const string SectionName = "AgentFramework";

    public OllamaOptions Ollama { get; set; } = new();
}

public class OllamaOptions
{
    public string Endpoint { get; set; } = "http://localhost:11434";
    public string Model { get; set; } = "gpt-oss:20b";
}
```

#### 4. Registrar Agent Framework no DI
```csharp
// AutorLLM.Infrastructure/DependencyInjection.cs
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;

public static IServiceCollection AddAgentFramework(
    this IServiceCollection services, 
    IConfiguration configuration)
{
    // Bind configuration
    services.Configure<AgentFrameworkOptions>(
        configuration.GetSection(AgentFrameworkOptions.SectionName)
    );

    // Register IChatClient (Ollama)
    services.AddSingleton<IChatClient>(sp =>
    {
        var options = sp.GetRequiredService<IOptions<AgentFrameworkOptions>>().Value;
        var logger = sp.GetRequiredService<ILogger<IChatClient>>();

        return new OllamaChatClient(
            new Uri(options.Ollama.Endpoint),
            options.Ollama.Model
        );
    });

    // Register AIAgent Factory
    services.AddSingleton<Func<string, AIAgent>>(sp =>
    {
        var chatClient = sp.GetRequiredService<IChatClient>();
        
        return (instructions) => chatClient.AsAIAgent(
            instructions: instructions,
            name: "AutorLLM Assistant"
        );
    });

    return services;
}
```

#### 5. Criar AgentService
```csharp
// AutorLLM.Application/Services/IAgentService.cs
namespace AutorLLM.Application.Services;

public interface IAgentService
{
    IAsyncEnumerable<string> StreamCompletionAsync(
        string prompt,
        CancellationToken cancellationToken = default
    );

    Task<string> CompleteAsync(
        string prompt,
        CancellationToken cancellationToken = default
    );

    Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default);
}
```

```csharp
// AutorLLM.Infrastructure/Services/AgentService.cs
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AutorLLM.Infrastructure.Services;

public class AgentService : IAgentService
{
    private readonly AIAgent _agent;
    private readonly ILogger<AgentService> _logger;

    public AgentService(
        Func<string, AIAgent> agentFactory, 
        ILogger<AgentService> logger)
    {
        _agent = agentFactory("You are a creative writing assistant. Help authors improve their narrative text.");
        _logger = logger;
    }

    public async IAsyncEnumerable<string> StreamCompletionAsync(
        string prompt,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Streaming completion for prompt length: {Length}", prompt.Length);

        await foreach (var update in _agent.RunStreamingAsync(prompt, cancellationToken: cancellationToken))
        {
            var text = update.Text;
            if (!string.IsNullOrEmpty(text))
            {
                yield return text;
            }
        }
    }

    public async Task<string> CompleteAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Running completion for prompt length: {Length}", prompt.Length);
        
        var response = await _agent.RunAsync(prompt, cancellationToken: cancellationToken);
        return response.Text;
    }

    public async Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _agent.RunAsync(
                "Say 'OK' if you can read this.", 
                cancellationToken: cancellationToken
            );
            
            return result.Text.Contains("OK", StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return false;
        }
    }
}
```

#### 6. Remover ILLMService e atualizar LLMHub
```csharp
// AutorLLM.Api/Hubs/LLMHub.cs
using Microsoft.AspNetCore.SignalR;
using AutorLLM.Application.Services;

namespace AutorLLM.Api.Hubs;

public class LLMHub : Hub
{
    private readonly IAgentService _agentService;
    private readonly ILogger<LLMHub> _logger;

    public LLMHub(IAgentService agentService, ILogger<LLMHub> logger)
    {
        _agentService = agentService;
        _logger = logger;
    }

    public async Task RequestRewrite(string chapterId, string selectedText, string command)
    {
        _logger.LogInformation(
            "Received rewrite request for chapter {ChapterId}. Command: {Command}",
            chapterId,
            command
        );

        try
        {
            var prompt = BuildPrompt(selectedText, command);

            await foreach (var token in _agentService.StreamCompletionAsync(prompt, Context.ConnectionAborted))
            {
                await Clients.Caller.SendAsync("OnTokenReceived", token);
            }

            await Clients.Caller.SendAsync("OnComplete");
            
            _logger.LogInformation("Rewrite request completed for chapter {ChapterId}", chapterId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing rewrite request for chapter {ChapterId}", chapterId);
            await Clients.Caller.SendAsync("OnError", ex.Message);
        }
    }

    private static string BuildPrompt(string selectedText, string command)
    {
        return $"""
            Você é um assistente de escrita criativa.
            
            Texto selecionado:
            {selectedText}
            
            Instrução:
            {command}
            
            Reescreva o texto seguindo a instrução fornecida, mantendo coerência narrativa.
            """;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception != null)
        {
            _logger.LogError(exception, "Client disconnected with error: {ConnectionId}", Context.ConnectionId);
        }
        else
        {
            _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}
```

---

## US073: Implementar Streaming

### Objetivo
Processar streaming de tokens via Microsoft Agent Framework.

### Implementação
- ✅ Já implementado na US072 usando `RunStreamingAsync()`
- ✅ AIAgent retorna `IAsyncEnumerable<AgentResponseUpdate>`
- ✅ Cada update tem `.Text` com os tokens incrementais
- ✅ SignalR envia cada token via `Clients.Caller.SendAsync("OnTokenReceived", token)`

### Exemplo de Uso
```csharp
// Streaming automático via AIAgent
await foreach (var update in _agent.RunStreamingAsync(prompt, cancellationToken))
{
    // update.Text contém o texto incremental
    if (!string.IsNullOrEmpty(update.Text))
    {
        await Clients.Caller.SendAsync("OnTokenReceived", update.Text);
    }
}
```

---

## US074: Tratar Erros

### Objetivo
Implementar error handling robusto com retry policies usando Polly.

### Implementação

#### 1. Adicionar Polly para Retry
```bash
dotnet add package Polly --version 8.x
dotnet add package Microsoft.Extensions.Http.Resilience --version 8.x
```

#### 2. Configurar Resilience no IChatClient
```csharp
// AutorLLM.Infrastructure/DependencyInjection.cs
using Microsoft.Extensions.Http.Resilience;
using Polly;

public static IServiceCollection AddAgentFramework(
    this IServiceCollection services, 
    IConfiguration configuration)
{
    // ... código anterior ...

    // Register IChatClient com retry policy
    services.AddSingleton<IChatClient>(sp =>
    {
        var options = sp.GetRequiredService<IOptions<AgentFrameworkOptions>>().Value;
        var logger = sp.GetRequiredService<ILogger<IChatClient>>();

        var baseClient = new OllamaChatClient(
            new Uri(options.Ollama.Endpoint),
            options.Ollama.Model
        );

        // Wrap com retry policy usando ConfigureHttpClientDefaults
        return baseClient;
    });

    return services;
}
```

#### 3. Implementar Circuit Breaker no AgentService
```csharp
// AutorLLM.Infrastructure/Services/AgentService.cs
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

public class AgentService : IAgentService
{
    private readonly AIAgent _agent;
    private readonly ILogger<AgentService> _logger;
    private readonly ResiliencePipeline _resiliencePipeline;

    public AgentService(
        Func<string, AIAgent> agentFactory, 
        ILogger<AgentService> logger)
    {
        _agent = agentFactory("You are a creative writing assistant.");
        _logger = logger;

        // Retry policy: 3 tentativas com exponential backoff
        var retryOptions = new RetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromSeconds(1),
            BackoffType = DelayBackoffType.Exponential,
            OnRetry = args =>
            {
                _logger.LogWarning(
                    "Retry {Attempt} after {Delay}ms due to {Exception}",
                    args.AttemptNumber, 
                    args.RetryDelay.TotalMilliseconds,
                    args.Outcome.Exception?.Message
                );
                return ValueTask.CompletedTask;
            }
        };

        // Circuit breaker: abre após 3 falhas consecutivas
        var circuitBreakerOptions = new CircuitBreakerStrategyOptions
        {
            FailureRatio = 0.5,
            SamplingDuration = TimeSpan.FromSeconds(30),
            MinimumThroughput = 3,
            BreakDuration = TimeSpan.FromMinutes(1),
            OnOpened = args =>
            {
                _logger.LogError("Circuit breaker opened");
                return ValueTask.CompletedTask;
            },
            OnClosed = args =>
            {
                _logger.LogInformation("Circuit breaker closed");
                return ValueTask.CompletedTask;
            }
        };

        _resiliencePipeline = new ResiliencePipelineBuilder()
            .AddRetry(retryOptions)
            .AddCircuitBreaker(circuitBreakerOptions)
            .AddTimeout(TimeSpan.FromSeconds(60))
            .Build();
    }

    public async IAsyncEnumerable<string> StreamCompletionAsync(
        string prompt,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var update in _resiliencePipeline.ExecuteAsync(
            async ct => _agent.RunStreamingAsync(prompt, cancellationToken: ct),
            cancellationToken))
        {
            var text = update.Text;
            if (!string.IsNullOrEmpty(text))
            {
                yield return text;
            }
        }
    }
}
```

#### 4. Tratamento de Erros no LLMHub
```csharp
public async Task RequestRewrite(string chapterId, string selectedText, string command)
{
    try
    {
        var prompt = BuildPrompt(selectedText, command);

        await foreach (var token in _agentService.StreamCompletionAsync(prompt, Context.ConnectionAborted))
        {
            await Clients.Caller.SendAsync("OnTokenReceived", token);
        }

        await Clients.Caller.SendAsync("OnComplete");
    }
    catch (BrokenCircuitException ex)
    {
        _logger.LogError(ex, "Circuit breaker is open");
        await Clients.Caller.SendAsync("OnError", "O serviço LLM está temporariamente indisponível. Tente novamente em alguns minutos.");
    }
    catch (TimeoutRejectedException ex)
    {
        _logger.LogError(ex, "Request timeout");
        await Clients.Caller.SendAsync("OnError", "A requisição demorou muito tempo. Por favor, tente com um texto menor.");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unexpected error");
        await Clients.Caller.SendAsync("OnError", "LLM não disponível. Verifique se o Ollama está rodando.");
    }
}
```

---

## US075: Configurar Modelo

### Objetivo
Permitir troca de modelo via appsettings.json.

### Implementação
- ✅ Já implementado na US072 via `AgentFrameworkOptions`
- ✅ Modelo configurável em `appsettings.json`
- ✅ IChatClient criado com modelo especificado

### Modelos Suportados
- `gpt-oss:20b` (padrão - já baixado via `ollama pull gpt-oss:20b`)
- `llama3.1:70b` (se hardware permitir)
- `qwen2.5:32b` (alternativa)
- Qualquer modelo disponível no Ollama local

### Configuração
```json
{
  "AgentFramework": {
    "Ollama": {
      "Endpoint": "http://localhost:11434",
      "Model": "gpt-oss:20b"  // ← Modelo configurável
    }
  }
}
```

### Trocar Modelo
Para trocar o modelo, basta:
1. Baixar o modelo desejado: `ollama pull <modelo>`
2. Atualizar `appsettings.json` com o nome do modelo
3. Reiniciar a aplicação

---

## Testes Unitários

```csharp
// AutorLLM.Tests/Unit/Infrastructure/AgentServiceTests.cs
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Moq;

public class AgentServiceTests
{
    private readonly Mock<ILogger<AgentService>> _loggerMock;
    private readonly Mock<AIAgent> _agentMock;

    public AgentServiceTests()
    {
        _loggerMock = new Mock<ILogger<AgentService>>();
        _agentMock = new Mock<AIAgent>();
    }

    [Fact]
    public async Task StreamCompletionAsync_ShouldYieldTokens()
    {
        // Arrange
        var agentFactory = new Func<string, AIAgent>(_ => _agentMock.Object);
        var service = new AgentService(agentFactory, _loggerMock.Object);

        var updates = new List<AgentResponseUpdate>
        {
            new AgentResponseUpdate { Text = "Token1 " },
            new AgentResponseUpdate { Text = "Token2 " },
            new AgentResponseUpdate { Text = "Token3" }
        };

        _agentMock
            .Setup(a => a.RunStreamingAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(updates.ToAsyncEnumerable());

        // Act
        var tokens = new List<string>();
        await foreach (var token in service.StreamCompletionAsync("test prompt"))
        {
            tokens.Add(token);
        }

        // Assert
        tokens.Should().HaveCount(3);
        tokens.Should().ContainInOrder("Token1 ", "Token2 ", "Token3");
    }

    [Fact]
    public async Task CompleteAsync_ShouldReturnText()
    {
        // Arrange
        var agentFactory = new Func<string, AIAgent>(_ => _agentMock.Object);
        var service = new AgentService(agentFactory, _loggerMock.Object);

        var response = new AgentResponse { Text = "Complete response" };
        _agentMock
            .Setup(a => a.RunAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await service.CompleteAsync("test prompt");

        // Assert
        result.Should().Be("Complete response");
    }

    [Fact]
    public async Task HealthCheckAsync_ShouldReturnTrue_WhenOllamaResponds()
    {
        // Arrange
        var agentFactory = new Func<string, AIAgent>(_ => _agentMock.Object);
        var service = new AgentService(agentFactory, _loggerMock.Object);

        var response = new AgentResponse { Text = "OK, I can read this." };
        _agentMock
            .Setup(a => a.RunAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await service.HealthCheckAsync();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task HealthCheckAsync_ShouldReturnFalse_WhenExceptionOccurs()
    {
        // Arrange
        var agentFactory = new Func<string, AIAgent>(_ => _agentMock.Object);
        var service = new AgentService(agentFactory, _loggerMock.Object);

        _agentMock
            .Setup(a => a.RunAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Connection failed"));

        // Act
        var result = await service.HealthCheckAsync();

        // Assert
        result.Should().BeFalse();
    }
}
```

---

## Checklist de Implementação

### US072: Conectar Backend ao Ollama
- [ ] Adicionar pacotes NuGet (Microsoft.Extensions.AI, Microsoft.Extensions.AI.Ollama, Microsoft.Agents.AI)
- [ ] Criar `AgentFrameworkOptions` e configurar appsettings.json
- [ ] Registrar IChatClient (Ollama) no DI
- [ ] Criar factory de AIAgent
- [ ] Criar `IAgentService` e `AgentService`
- [ ] Remover `ILLMService` do código
- [ ] Atualizar `LLMHub` para usar `IAgentService`
- [ ] Criar testes unitários
- [ ] Testar health check com Ollama rodando (modelo gpt-oss:20b)

### US073: Implementar Streaming
- [ ] Validar que streaming funciona via AIAgent.RunStreamingAsync()
- [ ] Testar com diferentes prompts
- [ ] Validar performance (tokens/segundo)
- [ ] Testar cancelamento via CancellationToken
- [ ] Confirmar que AgentResponseUpdate.Text retorna tokens incrementais

### US074: Tratar Erros
- [ ] Adicionar Polly v8 para retry policies
- [ ] Adicionar Microsoft.Extensions.Http.Resilience
- [ ] Implementar ResiliencePipeline com retry + circuit breaker + timeout
- [ ] Configurar timeout (60s)
- [ ] Testar com Ollama offline
- [ ] Validar logs de erro
- [ ] Criar testes para cenários de falha (BrokenCircuitException, TimeoutRejectedException)

### US075: Configurar Modelo
- [ ] Validar configuração via appsettings.json
- [ ] Testar troca de modelo (gpt-oss-20b → llama3.1:70b)
- [ ] Documentar modelos suportados
- [ ] Criar testes de configuração

---

## Referências

- **[Microsoft Agent Framework - Quick Start](https://learn.microsoft.com/en-us/agent-framework/tutorials/quick-start?pivots=programming-language-csharp)**
- **[Microsoft Agent Framework - Create and Run Agent](https://learn.microsoft.com/en-us/agent-framework/tutorials/agents/run-agent)**
- [Microsoft.Extensions.AI Documentation](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.ai)
- [IChatClient Interface](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.ai.ichatclient)
- [Ollama API Documentation](https://github.com/ollama/ollama/blob/main/docs/api.md)
- [Polly v8 Documentation](https://www.pollydocs.org/)
