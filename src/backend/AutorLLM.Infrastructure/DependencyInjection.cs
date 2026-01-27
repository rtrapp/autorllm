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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using OpenAI;
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

        // Register AIAgent usando OpenAI client (Ollama é compatível com OpenAI API)
        services.AddSingleton<AIAgent>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AgentFrameworkOptions>>().Value;
            var logger = sp.GetRequiredService<ILogger<AIAgent>>();

            logger.LogInformation(
                "Initializing AIAgent with Ollama endpoint: {Endpoint}, model: {Model}",
                options.Ollama.Endpoint,
                options.Ollama.Model
            );

            // Ollama é compatível com OpenAI API
            var openAIClient = new OpenAIClient(
                credential: new System.ClientModel.ApiKeyCredential("ollama"), // Ollama não requer API key real
                options: new OpenAIClientOptions
                {
                    Endpoint = new Uri(options.Ollama.Endpoint)
                }
            );

            // Padrão oficial: ChatClient → AsIChatClient() → AsAIAgent()
            return openAIClient
                .GetChatClient(options.Ollama.Model)
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

