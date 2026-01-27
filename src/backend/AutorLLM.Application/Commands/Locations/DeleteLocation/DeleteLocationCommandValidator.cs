using FluentValidation;

namespace AutorLLM.Application.Commands.Locations.DeleteLocation;

/// <summary>
/// Validator for DeleteLocationCommand
/// </summary>
public class DeleteLocationCommandValidator : AbstractValidator<DeleteLocationCommand>
{
    public DeleteLocationCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("ProjectId is required");

        RuleFor(x => x.LocationId)
            .NotEmpty().WithMessage("LocationId is required");
    }
}
