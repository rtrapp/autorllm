using FluentValidation;

namespace AutorLLM.Application.Commands.Chapters.ReorderChapters;

/// <summary>
/// Validator for ReorderChaptersCommand
/// </summary>
public class ReorderChaptersCommandValidator : AbstractValidator<ReorderChaptersCommand>
{
    public ReorderChaptersCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("ProjectId is required");

        RuleFor(x => x.ChapterIds)
            .NotEmpty()
            .WithMessage("ChapterIds list cannot be empty");
    }
}
