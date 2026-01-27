using FluentValidation;

namespace AutorLLM.Application.Commands.Locations.UpdateLocation;

/// <summary>
/// Validator for UpdateLocationCommand
/// </summary>
public class UpdateLocationCommandValidator : AbstractValidator<UpdateLocationCommand>
{
    public UpdateLocationCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("ProjectId is required");

        RuleFor(x => x.LocationId)
            .NotEmpty().WithMessage("LocationId is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must be under 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must be under 1000 characters");

        RuleFor(x => x.Geography)
            .MaximumLength(2000).WithMessage("Geography must be under 2000 characters")
            .When(x => x.Geography != null);

        RuleFor(x => x.Culture)
            .MaximumLength(2000).WithMessage("Culture must be under 2000 characters")
            .When(x => x.Culture != null);

        RuleFor(x => x.Significance)
            .MaximumLength(1000).WithMessage("Significance must be under 1000 characters")
            .When(x => x.Significance != null);
    }
}
