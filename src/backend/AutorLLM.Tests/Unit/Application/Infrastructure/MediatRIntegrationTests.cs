using AutorLLM.Application;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AutorLLM.Tests.Unit.Application.Infrastructure;

/// <summary>
/// Tests for MediatR integration and DI configuration
/// </summary>
public class MediatRIntegrationTests
{
    [Fact]
    public void AddApplication_ShouldRegisterMediatR()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplication();

        // Assert - Check that MediatR infrastructure is registered
        var mediatRServices = services
            .Where(s => s.ServiceType == typeof(IMediator) ||
                       s.ServiceType == typeof(ISender) ||
                       s.ServiceType == typeof(IPublisher))
            .ToList();

        mediatRServices.Should().NotBeEmpty("MediatR services (IMediator, ISender, IPublisher) should be registered");
    }

    [Fact]
    public void AddApplication_ShouldRegisterValidators()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplication();

        // Assert - Check that validators are registered
        var validators = services
            .Where(s => s.ServiceType.IsGenericType &&
                       s.ServiceType.GetGenericTypeDefinition() == typeof(IValidator<>))
            .ToList();

        validators.Should().NotBeEmpty("FluentValidation validators should be registered automatically");
    }

    [Fact]
    public void AddApplication_ShouldRegisterValidationBehavior()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplication();

        // Assert - Check that pipeline behaviors are registered
        var behaviorServices = services
            .Where(s => s.ServiceType.IsGenericType &&
                       s.ServiceType.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>))
            .ToList();

        behaviorServices.Should().NotBeEmpty("ValidationBehavior should be registered as pipeline behavior");
    }
}
