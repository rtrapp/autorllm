using FluentValidation;

namespace AutorLLM.Application.Commands.PlotPoints.UpdatePlotPoint;

/// <summary>
/// Validator for UpdatePlotPointCommand
/// </summary>
public class UpdatePlotPointCommandValidator : AbstractValidator<UpdatePlotPointCommand>
{
    public UpdatePlotPointCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("ProjectId is required");

        RuleFor(x => x.PlotId)
            .NotEmpty()
            .WithMessage("PlotId is required");

        RuleFor(x => x.PlotPointId)
            .NotEmpty()
            .WithMessage("PlotPointId is required");

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
