using FluentValidation;

namespace AutorLLM.Application.Commands.PlotPoints.DeletePlotPoint;

/// <summary>
/// Validator for DeletePlotPointCommand
/// </summary>
public class DeletePlotPointCommandValidator : AbstractValidator<DeletePlotPointCommand>
{
    public DeletePlotPointCommandValidator()
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
    }
}
