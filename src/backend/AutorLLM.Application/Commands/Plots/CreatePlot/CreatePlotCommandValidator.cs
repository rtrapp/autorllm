using FluentValidation;

namespace AutorLLM.Application.Commands.Plots.CreatePlot;

/// <summary>
/// Validator for CreatePlotCommand
/// </summary>
public class CreatePlotCommandValidator : AbstractValidator<CreatePlotCommand>
{
    public CreatePlotCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("ProjectId is required");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must be under 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must be under 2000 characters");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Type is required")
            .MaximumLength(50).WithMessage("Type must be under 50 characters");
    }
}
