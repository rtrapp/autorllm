using FluentValidation;

namespace AutorLLM.Application.Commands.Plots.UpdatePlot;

/// <summary>
/// Validator for UpdatePlotCommand
/// </summary>
public class UpdatePlotCommandValidator : AbstractValidator<UpdatePlotCommand>
{
    public UpdatePlotCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("ProjectId is required");

        RuleFor(x => x.PlotId)
            .NotEmpty().WithMessage("PlotId is required");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must be under 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must be under 2000 characters");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Type is required")
            .MaximumLength(50).WithMessage("Type must be under 50 characters");

        RuleFor(x => x.Resolution)
            .MaximumLength(2000).WithMessage("Resolution must be under 2000 characters");
    }
}
