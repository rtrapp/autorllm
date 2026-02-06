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

            Console.WriteLine("Timeout value {0}", timeout);

            // Circuit breaker
            resilienceOptions.CircuitBreaker = new HttpCircuitBreakerStrategyOptions
            {
                FailureRatio = 0.5,
                MinimumThroughput = options.Resilience.CircuitBreakerFailureThreshold,
                BreakDuration = TimeSpan.FromSeconds(timeout * 2),
                SamplingDuration = TimeSpan.FromSeconds(timeout * 2),

            };

            // Timeout (já configurado no HttpClient, mas reforçando via resilience)
            resilienceOptions.TotalRequestTimeout = new HttpTimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromSeconds(timeout)
            };

            // Timeout per attempt (importante para LLMs lentos)
            resilienceOptions.AttemptTimeout = new HttpTimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromSeconds(timeout)
            };
        });

        // Register AIAgent usando OpenAI client (compatível com Ollama e LM Studio)
        services.AddSingleton<IChatClient>(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<AgentFrameworkOptions>>().Value;
            var logger = sp.GetRequiredService<ILogger<IChatClient>>();
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

            // Register ChatClient from OpenAI SDK (compatible with Ollama) as IChatClient
            var chatClient = openAIClient.GetChatClient(activeModel).AsIChatClient();
            
            return chatClient;
        });

        // Register Agent Definitions
        services.AddSingleton<AutorLLM.Application.AgentDefinitions.BrainstormAgentDefinition>();

        // Register AgentService
        services.AddScoped<IAgentService, AgentService>();

        return services;
    }
}

