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
    /// Adds Microsoft Agent Framework with Ollama integration
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

        // Configure HttpClient with resilience policies
        services.AddHttpClient("OllamaClient", client =>
        {
            client.BaseAddress = new Uri(options.Ollama.Endpoint);
            client.Timeout = TimeSpan.FromSeconds(options.Ollama.TimeoutSeconds);
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
                Timeout = TimeSpan.FromSeconds(options.Ollama.TimeoutSeconds)
            };
        });

        // Register AIAgent usando OpenAI client (Ollama é compatível com OpenAI API)
        services.AddSingleton<AIAgent>(sp =>
        {
            var opts = sp.GetRequiredService<IOptions<AgentFrameworkOptions>>().Value;
            var logger = sp.GetRequiredService<ILogger<AIAgent>>();
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();

            logger.LogInformation(
                "Initializing AIAgent with Ollama endpoint: {Endpoint}, model: {Model}",
                opts.Ollama.Endpoint,
                opts.Ollama.Model
            );

            // Usar HttpClient configurado com resilience
            var httpClient = httpClientFactory.CreateClient("OllamaClient");

            // Ollama é compatível com OpenAI API
            var openAIClient = new OpenAIClient(
                credential: new System.ClientModel.ApiKeyCredential("ollama"), // Ollama não requer API key real
                options: new OpenAIClientOptions
                {
                    Endpoint = new Uri(opts.Ollama.Endpoint),
                    Transport = new System.ClientModel.Primitives.HttpClientPipelineTransport(httpClient)
                }
            );

            // Padrão oficial: ChatClient → AsIChatClient() → AsAIAgent()
            return openAIClient
                .GetChatClient(opts.Ollama.Model)
                .AsIChatClient()
                .AsAIAgent(
                    instructions: "Você é um assistente de escrita criativa. Ajude autores a melhorar seus textos narrativos.",
                    name: "AutorLLM Assistant"
                );
        });

        // Register AgentService
        services.AddScoped<IAgentService, AgentService>();

        return services;
    }
}

