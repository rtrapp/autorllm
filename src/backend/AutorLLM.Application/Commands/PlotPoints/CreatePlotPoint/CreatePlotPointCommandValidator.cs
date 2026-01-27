using FluentValidation;

namespace AutorLLM.Application.Commands.PlotPoints.CreatePlotPoint;

/// <summary>
/// Validator for CreatePlotPointCommand
/// </summary>
public class CreatePlotPointCommandValidator : AbstractValidator<CreatePlotPointCommand>
{
    public CreatePlotPointCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("ProjectId is required");

        RuleFor(x => x.PlotId)
            .NotEmpty()
            .WithMessage("PlotId is required");

        RuleFor(x => x.ChapterId)
            .NotEmpty()
            .WithMessage("ChapterId is required");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(500)
            .WithMessage("Description must be under 500 characters");

        RuleFor(x => x.Intensity)
            .InclusiveBetween(0, 10)
            .WithMessage("Intensity must be between 0 and 10");
    }
}
