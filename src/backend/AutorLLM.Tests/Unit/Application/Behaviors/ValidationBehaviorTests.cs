using AutorLLM.Application.Behaviors;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;

namespace AutorLLM.Tests.Unit.Application.Behaviors;

/// <summary>
/// Tests for ValidationBehavior pipeline
/// </summary>
public class ValidationBehaviorTests
{
    // Test request and response types - must be public for Moq to create proxies
    public record TestRequest(string Name) : IRequest<TestResponse>;
    public record TestResponse(string Result);

    [Fact]
    public async Task Handle_WhenNoValidators_ShouldProceedToNextBehavior()
    {
        // Arrange
        var request = new TestRequest("Test");
        var expectedResponse = new TestResponse("Success");
        var nextCalled = false;

        var behavior = new ValidationBehavior<TestRequest, TestResponse>(
            Enumerable.Empty<IValidator<TestRequest>>());

        // Act
        var result = await behavior.Handle(request, (ct) =>
        {
            nextCalled = true;
            return Task.FromResult(expectedResponse);
        }, CancellationToken.None);

        // Assert
        result.Should().Be(expectedResponse, "behavior should proceed when no validators exist");
        nextCalled.Should().BeTrue("next delegate should be called");
    }

    [Fact]
    public async Task Handle_WhenValidationPasses_ShouldProceedToNextBehavior()
    {
        // Arrange
        var request = new TestRequest("ValidName");
        var expectedResponse = new TestResponse("Success");
        var nextCalled = false;

        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var behavior = new ValidationBehavior<TestRequest, TestResponse>(new[] { validatorMock.Object });

        // Act
        var result = await behavior.Handle(request, (ct) =>
        {
            nextCalled = true;
            return Task.FromResult(expectedResponse);
        }, CancellationToken.None);

        // Assert
        result.Should().Be(expectedResponse, "behavior should proceed when validation passes");
        nextCalled.Should().BeTrue("next delegate should be called");
        validatorMock.Verify(v => v.ValidateAsync(
            It.IsAny<ValidationContext<TestRequest>>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenValidationFails_ShouldThrowValidationException()
    {
        // Arrange
        var request = new TestRequest("");
        var expectedResponse = new TestResponse("Success");

        var validationFailure = new ValidationFailure("Name", "Name is required");
        var validationResult = new ValidationResult(new[] { validationFailure });

        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var behavior = new ValidationBehavior<TestRequest, TestResponse>(new[] { validatorMock.Object });

        // Act
        var act = async () => await behavior.Handle(request, (ct) => Task.FromResult(expectedResponse), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>("validation should throw when it fails");
        validatorMock.Verify(v => v.ValidateAsync(
            It.IsAny<ValidationContext<TestRequest>>(),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenMultipleValidatorsFail_ShouldCollectAllErrors()
    {
        // Arrange
        var request = new TestRequest("");
        var expectedResponse = new TestResponse("Success");

        var validator1Mock = new Mock<IValidator<TestRequest>>();
        validator1Mock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Name", "Error 1") }));

        var validator2Mock = new Mock<IValidator<TestRequest>>();
        validator2Mock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Name", "Error 2") }));

        var behavior = new ValidationBehavior<TestRequest, TestResponse>(
            new[] { validator1Mock.Object, validator2Mock.Object });

        // Act
        var act = async () => await behavior.Handle(request, (ct) => Task.FromResult(expectedResponse), CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<ValidationException>();
        exception.Which.Errors.Should().HaveCount(2, "all validation errors should be collected");
    }
}
