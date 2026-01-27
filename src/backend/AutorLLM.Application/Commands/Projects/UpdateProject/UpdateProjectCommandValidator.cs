using FluentValidation;

namespace AutorLLM.Application.Commands.Projects.UpdateProject;

/// <summary>
/// Validator for UpdateProjectCommand
/// </summary>
public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("ProjectId is required");

        RuleFor(x => x.Title)
            .MaximumLength(200).WithMessage("Title must be under 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.Author)
            .MaximumLength(100).WithMessage("Author must be under 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Author));

        RuleFor(x => x.Synopsis)
            .MaximumLength(5000).WithMessage("Synopsis must be under 5000 characters")
            .When(x => !string.IsNullOrEmpty(x.Synopsis));

        RuleFor(x => x.Genre)
            .MaximumLength(50).WithMessage("Genre must be under 50 characters")
            .When(x => !string.IsNullOrEmpty(x.Genre));

        RuleFor(x => x.TargetWordCount)
            .GreaterThanOrEqualTo(0).WithMessage("Target word count must be greater than or equal to 0")
            .When(x => x.TargetWordCount.HasValue);
    }
}
