using FluentValidation;

namespace AutorLLM.Application.Commands.Locations.CreateLocation;

/// <summary>
/// Validator for CreateLocationCommand
/// </summary>
public class CreateLocationCommandValidator : AbstractValidator<CreateLocationCommand>
{
    public CreateLocationCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("ProjectId is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must be under 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must be under 1000 characters");
    }
}
