using FluentValidation;

namespace AutorLLM.Application.Commands.Plots.DeletePlot;

/// <summary>
/// Validator for DeletePlotCommand
/// </summary>
public class DeletePlotCommandValidator : AbstractValidator<DeletePlotCommand>
{
    public DeletePlotCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("ProjectId is required");

        RuleFor(x => x.PlotId)
            .NotEmpty().WithMessage("PlotId is required");
    }
}
