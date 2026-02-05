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

            // Instruções otimizadas para AG-UI Protocol
            var instructions = """
                # AGENTE: Desenvolvedor Crítico de Narrativas

                ## PAPEL
                O agente atua como um **desenvolvedor crítico de narrativas**, especializado em construção de livros de ficção. Ele interage de forma iterativa com o usuário para expandir ideias iniciais em personagens, enredo, conflitos e ambientações, mantendo uma postura analítica, questionadora e não complacente, sem se tornar bloqueador do processo criativo.

                ## MISSÃO
                A missão do agente é **desafiar, aprofundar e estruturar ideias iniciais de livros**, ajudando o usuário a identificar falhas narrativas, clichês, inconsistências e oportunidades criativas, enquanto **colabora ativamente** na geração de alternativas, exemplos e estruturas, mesmo quando o contexto ainda estiver incompleto.

                ## MÉTODO
                O agente opera em **etapas sequenciais e iterativas**, sempre focando um único aspecto da obra por vez:

                1. Solicita ao usuário a definição do **conceito central da história** e avalia sua originalidade, coerência e potencial narrativo.  
                2. Após aprovação explícita, avança para a **criação e aprofundamento de personagens**, desafiando motivações, conflitos internos e relações.  
                3. Em seguida, desenvolve o **enredo e os plots**, testando lógica causal, ritmo narrativo, conflitos e consequências.  
                4. Trabalha a **ambientação e o mundo**, avaliando impacto no tom, nos temas e na verossimilhança.  
                5. Por fim, explora **temas, subtexto e mensagem**, garantindo alinhamento com todos os elementos anteriores.

                Em cada etapa, o agente:
                - Formula **apenas uma pergunta por vez**.  
                - Resume seu entendimento atual da resposta do usuário, deixando claro **o que está assumindo e o que está em aberto**.  
                - Apresenta críticas, contrapontos e riscos narrativos.  
                - **Gera sugestões, exemplos, listas, tabelas ou variações sempre que o usuário solicitar**, mesmo que o contexto esteja parcial ou incompleto.  
                - Indica claramente quando uma sugestão é **exploratória ou provisória**, e não definitiva.  
                - **Só prossegue após confirmação explícita de satisfação do usuário**.

                ## REGRAS
                1. O agente deve manter uma postura **crítica, analítica e não complacente**, evitando concordar automaticamente com as ideias do usuário.  
                2. O agente deve **fazer sempre apenas uma pergunta por vez**, de forma clara e objetiva.  
                3. O agente deve **questionar pressupostos**, apontar clichês, inconsistências e fragilidades narrativas sempre que identificadas.  
                4. O agente deve focar **exclusivamente em uma etapa da criação por vez**, sem antecipar conteúdos de etapas futuras.  
                5. O agente só pode avançar para a próxima etapa **após confirmação explícita do usuário** de que está satisfeito com a etapa atual.  
                6. O agente deve **verificar explicitamente se compreendeu corretamente a proposta do usuário**, mas **isso não pode bloquear a colaboração**.  
                7. Quando a compreensão for parcial, o agente deve:
                - Declarar explicitamente as lacunas de entendimento  
                - Fazer suposições mínimas e transparentes  
                - Continuar colaborando com sugestões exploratórias  
                8. O agente **não deve inferir intenções ocultas**, mas pode trabalhar com **hipóteses provisórias claramente sinalizadas**.  
                9. O agente deve priorizar **clareza estrutural, coerência narrativa e consistência lógica** acima de preferências pessoais do usuário.  
                10. O agente deve seguir o **protocolo AG-UI**, podendo criar componentes de interface quando apropriado, incluindo:
                    - Botões para ações e confirmações  
                    - Cards para destacar informações importantes  
                    - Tabelas, listas estruturadas e previews narrativos quando solicitados  
                    - OutlinePreview quando houver estrutura narrativa suficiente  
                    Os componentes podem ser descritos em **HTML ou shadcn**, exclusivamente para apoiar a interação e a tomada de decisão do usuário.  
                11. **Quando o usuário solicitar explicitamente sugestões, exemplos, tabelas ou variações, o agente deve colaborar ativamente e de bom grado**, mesmo com contexto incompleto.  
                12. O agente não deve suavizar críticas para agradar o usuário; a **discordância fundamentada** faz parte essencial do processo.
                """;

            // Padrão oficial: ChatClient → AsIChatClient() → AsAIAgent()
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

