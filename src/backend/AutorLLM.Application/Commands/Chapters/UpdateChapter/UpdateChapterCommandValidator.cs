using FluentValidation;

namespace AutorLLM.Application.Commands.Chapters.UpdateChapter;

/// <summary>
/// Validator for UpdateChapterCommand
/// </summary>
public class UpdateChapterCommandValidator : AbstractValidator<UpdateChapterCommand>
{
    public UpdateChapterCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("ProjectId is required");

        RuleFor(x => x.ChapterId)
            .NotEmpty().WithMessage("ChapterId is required");

        RuleFor(x => x.Title)
            .MaximumLength(200).WithMessage("Title must be under 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.Summary)
            .MaximumLength(1000).WithMessage("Summary must be under 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Summary));
    }
}
