using FluentValidation;

namespace AutorLLM.Application.Commands.Chapters.DeleteChapter;

/// <summary>
/// Validator for DeleteChapterCommand
/// </summary>
public class DeleteChapterCommandValidator : AbstractValidator<DeleteChapterCommand>
{
    public DeleteChapterCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("ProjectId is required");

        RuleFor(x => x.ChapterId)
            .NotEmpty()
            .WithMessage("ChapterId is required");
    }
}
