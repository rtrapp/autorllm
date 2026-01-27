using FluentValidation;

namespace AutorLLM.Application.Commands.Chapters.CreateChapter;

/// <summary>
/// Validator for CreateChapterCommand
/// </summary>
public class CreateChapterCommandValidator : AbstractValidator<CreateChapterCommand>
{
    public CreateChapterCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("ProjectId is required");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters");
    }
}
