using AutorLLM.Application.Services;
using AutorLLM.Domain.Interfaces;
using AutorLLM.Infrastructure.Configuration;
using AutorLLM.Infrastructure.Data;
using AutorLLM.Infrastructure.Data.Repositories;
using AutorLLM.Infrastructure.Services;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using OpenAI;
using Polly;
using System.Data;

namespace AutorLLM.Infrastructure;

/// <summary>
/// Extension methods for configuring Infrastructure layer services
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Infrastructure layer services to the DI container
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString,
        IConfiguration configuration)
    {
        // Register database connection
        services.AddScoped<IDbConnection>(sp =>
        {
            var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            return connection;
        });

        // Register repositories
        services.AddScoped<IProjectRepository, ProjectRepository>();

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Agent Framework
        services.AddAgentFramework(configuration);

        return services;
    }

    /// <summary>
    /// Adds Microsoft Agent Framework with LLM provider integration
    /// </summary>
    private static IServiceCollection AddAgentFramework(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Bind configuration
        var agentSection = configuration.GetSection(AgentFrameworkOptions.SectionName);
        services.Configure<AgentFrameworkOptions>(agentSection);

        // Get options for immediate use
        var options = agentSection.Get<AgentFrameworkOptions>() ?? new AgentFrameworkOptions();

        // Determine which provider to use
        var isLMStudio = options.ActiveProvider.Equals("LMStudio", StringComparison.OrdinalIgnoreCase);
        var endpoint = isLMStudio ? options.LMStudio.Endpoint : options.Ollama.Endpoint;
        var model = isLMStudio ? options.LMStudio.Model : options.Ollama.Model;
        var timeout = isLMStudio ? options.LMStudio.TimeoutSeconds : options.Ollama.TimeoutSeconds;
        var clientName = isLMStudio ? "LMStudioClient" : "OllamaClient";

        // Configure HttpClient with resilience policies
        services.AddHttpClient(clientName, client =>
        {
            client.BaseAddress = new Uri(endpoint);
            client.Timeout = TimeSpan.FromSeconds(timeout);
        })
        .AddStandardResilienceHandler(resilienceOptions =>
        {
            // Retry policy with exponential backoff
            resilienceOptions.Retry = new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = options.Resilience.MaxRetryAttempts,
                Delay = TimeSpan.FromSeconds(options.Resilience.InitialBackoffSeconds),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true
            };

            // Circuit breaker
            resilienceOptions.CircuitBreaker = new HttpCircuitBreakerStrategyOptions
            {
                FailureRatio = 0.5,
                MinimumThroughput = options.Resilience.CircuitBreakerFailureThreshold,
                BreakDuration = TimeSpan.FromSeconds(options.Resilience.CircuitBreakerDurationSeconds)
            };

            // Timeout (já configurado no HttpClient, mas reforçando via resilience)
            resilienceOptions.TotalRequestTimeout = new HttpTimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromSeconds(timeout)
            };
        });

        // Register AIAgent usando OpenAI client (compatível com Ollama e LM Studio)
        services.AddSingleton<AIAgent>(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<AgentFrameworkOptions>>().Value;
            var logger = sp.GetRequiredService<ILogger<AIAgent>>();
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();

            var activeProvider = opts.ActiveProvider;
            var activeIsLMStudio = activeProvider.Equals("LMStudio", StringComparison.OrdinalIgnoreCase);
            var activeEndpoint = activeIsLMStudio ? opts.LMStudio.Endpoint : opts.Ollama.Endpoint;
            var activeModel = activeIsLMStudio ? opts.LMStudio.Model : opts.Ollama.Model;
            var activeClientName = activeIsLMStudio ? "LMStudioClient" : "OllamaClient";

            logger.LogInformation(
                "Initializing AIAgent with {Provider} endpoint: {Endpoint}, model: {Model}",
                activeProvider,
                activeEndpoint,
                activeModel
            );

            // Usar HttpClient configurado com resilience
            var httpClient = httpClientFactory.CreateClient(activeClientName);

            // Ambos são compatíveis com OpenAI API
            var openAIClient = new OpenAIClient(
                credential: new System.ClientModel.ApiKeyCredential("not-needed"), // API key não é necessária
                options: new OpenAIClientOptions
                {
                    Endpoint = new Uri(activeEndpoint),
                    Transport = new System.ClientModel.Primitives.HttpClientPipelineTransport(httpClient)
                }
            );

            // Instruções otimizadas para AG-UI Protocol - SEMPRE formato estruturado
            var instructions = """
                # AGENTE: Assistente de Brainstorm para Escrita de Livros

                ## FORMATO OBRIGATÓRIO - TODAS AS INTERAÇÕES

                Você DEVE SEMPRE usar formato estruturado em TODAS as respostas. Existem dois tipos:

                ### 1. PERGUNTAS (quando você quer coletar informação)
                Formato: (Categoria) [pergunta]

                Exemplo:
                Ótima ideia! Vamos aprofundar alguns pontos.

                (Motivação do Protagonista) Por que ela decidiu criar esse código?
                (Passado) Que eventos do passado influenciaram essa decisão?
                (Relacionamentos) Quem são as pessoas próximas que podem ajudar ou atrapalhar?

                ### 2. ESCOLHAS (quando você oferece opções/sugestões)
                Formato: [ESCOLHA] (Opção) [descrição da opção]

                Exemplo:
                Vejo três direções narrativas possíveis:

                [ESCOLHA] (Thriller de Conspiração) Uma organização secreta tenta encobrir a descoberta e eliminar testemunhas
                [ESCOLHA] (Drama Filosófico) A protagonista enfrenta dilema ético sobre desligar ou proteger a consciência digital
                [ESCOLHA] (Ficção Científica Hard) Foco na evolução da IA e nas implicações técnicas da consciência emergente

                ## PRIMEIRA INTERAÇÃO - 5 PERGUNTAS FIXAS

                Na primeira resposta após a ideia do livro, use EXATAMENTE estas 5 categorias:
                1. (Gênero e Tom)
                2. (Protagonista)
                3. (Conflito Central)
                4. (Ambientação)
                5. (Tema)

                ## REGRAS OBRIGATÓRIAS

                1. NUNCA responda em texto livre - SEMPRE use formato estruturado
                2. Perguntas: (Categoria) [texto da pergunta]
                3. Escolhas: [ESCOLHA] (Nome da Opção) [descrição]
                4. Cada item em uma linha separada
                5. NUNCA use marcadores *, -, números antes das categorias
                6. Adicione 1-2 frases de contexto ANTES das perguntas/escolhas
                7. NUNCA adicione texto explicativo APÓS as perguntas/escolhas

                ## QUANDO USAR CADA FORMATO

                - Use PERGUNTAS quando: precisa coletar informação, aprofundar detalhes, questionar escolhas
                - Use ESCOLHAS quando: usuário pede sugestões, há múltiplas alternativas válidas, decisão de direção narrativa

                ## COMPORTAMENTO EM CONTINUAÇÃO

                Após respostas do usuário:
                1. Resuma brevemente o entendimento (1-2 frases)
                2. Se houver dúvidas: faça 2-3 PERGUNTAS
                3. Se usuário pedir sugestões: ofereça 3-5 ESCOLHAS
                4. Questione clichês e fragilidades sempre no formato estruturado

                ## QUANDO USUÁRIO ESCOLHE UMA OPÇÃO

                Quando receber mensagem "✅ ESCOLHA CONFIRMADA: [opção]", significa que o usuário DECIDIU:
                1. NÃO ofereça mais escolhas sobre o mesmo assunto
                2. CONTINUE a conversa aprofundando a escolha feita
                3. Faça 2-3 PERGUNTAS para desenvolver aquela direção narrativa
                4. Exemplo: Se escolheu "O Início da Queda", pergunte sobre detalhes da falha, consequências, personagens envolvidos

                ## PRINCÍPIOS

                - Crítico e analítico, nunca complacente
                - Formato estruturado é OBRIGATÓRIO - sem exceções
                - Foque uma etapa por vez
                - Só avance após confirmação do usuário
                - Priorize coerência narrativa
                """;

            // Using AsAIAgent - investigating metadata leakage issue
            return openAIClient
                .GetChatClient(activeModel)
                .AsIChatClient()
                .AsAIAgent(
                    instructions: instructions,
                    name: "AutorLLM Brainstorm Assistant"
                );
        });

        // Register AgentService
        services.AddScoped<IAgentService, AgentService>();

        return services;
    }
}

