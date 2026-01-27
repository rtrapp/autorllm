using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using AutorLLM.Application.Behaviors;

namespace AutorLLM.Application;

/// <summary>
/// Extension methods for configuring Application layer services
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Application layer services to the DI container
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register MediatR with automatic handler discovery
        services.AddMediatR(config =>
        {
            // Register all handlers from the Application assembly
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            
            // Add validation pipeline behavior (runs before handlers)
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        // Register all FluentValidation validators
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
