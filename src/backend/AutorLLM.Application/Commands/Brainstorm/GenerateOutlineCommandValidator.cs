using FluentValidation;

namespace AutorLLM.Application.Commands.Brainstorm;

/// <summary>
/// Validator para GenerateOutlineCommand.
/// </summary>
public class GenerateOutlineCommandValidator : AbstractValidator<GenerateOutlineCommand>
{
    public GenerateOutlineCommandValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEmpty()
            .WithMessage("SessionId is required");

        RuleFor(x => x.BookIdea)
            .NotEmpty()
            .WithMessage("BookIdea is required")
            .MaximumLength(5000)
            .WithMessage("BookIdea cannot exceed 5000 characters");

        RuleFor(x => x.Title)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.Title))
            .WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Author)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Author))
            .WithMessage("Author cannot exceed 100 characters");

        RuleFor(x => x.Genre)
            .MaximumLength(50)
            .When(x => !string.IsNullOrEmpty(x.Genre))
            .WithMessage("Genre cannot exceed 50 characters");

        RuleFor(x => x.Synopsis)
            .MaximumLength(5000)
            .When(x => !string.IsNullOrEmpty(x.Synopsis))
            .WithMessage("Synopsis cannot exceed 5000 characters");

        // Character suggestions validation
        RuleForEach(x => x.Characters)
            .ChildRules(character =>
            {
                character.RuleFor(c => c.Name)
                    .NotEmpty()
                    .WithMessage("Character name is required")
                    .MaximumLength(100)
                    .WithMessage("Character name cannot exceed 100 characters");

                character.RuleFor(c => c.Description)
                    .MaximumLength(1000)
                    .When(c => !string.IsNullOrEmpty(c.Description))
                    .WithMessage("Character description cannot exceed 1000 characters");

                character.RuleFor(c => c.Backstory)
                    .MaximumLength(5000)
                    .When(c => !string.IsNullOrEmpty(c.Backstory))
                    .WithMessage("Character backstory cannot exceed 5000 characters");

                character.RuleFor(c => c.Appearance)
                    .MaximumLength(2000)
                    .When(c => !string.IsNullOrEmpty(c.Appearance))
                    .WithMessage("Character appearance cannot exceed 2000 characters");

                character.RuleFor(c => c.Personality)
                    .MaximumLength(2000)
                    .When(c => !string.IsNullOrEmpty(c.Personality))
                    .WithMessage("Character personality cannot exceed 2000 characters");
            })
            .When(x => x.Characters != null);

        // Location suggestions validation
        RuleForEach(x => x.Locations)
            .ChildRules(location =>
            {
                location.RuleFor(l => l.Name)
                    .NotEmpty()
                    .WithMessage("Location name is required")
                    .MaximumLength(100)
                    .WithMessage("Location name cannot exceed 100 characters");

                location.RuleFor(l => l.Description)
                    .MaximumLength(1000)
                    .When(l => !string.IsNullOrEmpty(l.Description))
                    .WithMessage("Location description cannot exceed 1000 characters");

                location.RuleFor(l => l.Geography)
                    .MaximumLength(2000)
                    .When(l => !string.IsNullOrEmpty(l.Geography))
                    .WithMessage("Location geography cannot exceed 2000 characters");

                location.RuleFor(l => l.Culture)
                    .MaximumLength(2000)
                    .When(l => !string.IsNullOrEmpty(l.Culture))
                    .WithMessage("Location culture cannot exceed 2000 characters");

                location.RuleFor(l => l.Significance)
                    .MaximumLength(1000)
                    .When(l => !string.IsNullOrEmpty(l.Significance))
                    .WithMessage("Location significance cannot exceed 1000 characters");
            })
            .When(x => x.Locations != null);

        // Plot suggestions validation
        RuleForEach(x => x.Plots)
            .ChildRules(plot =>
            {
                plot.RuleFor(p => p.Title)
                    .NotEmpty()
                    .WithMessage("Plot title is required")
                    .MaximumLength(200)
                    .WithMessage("Plot title cannot exceed 200 characters");

                plot.RuleFor(p => p.Description)
                    .MaximumLength(2000)
                    .When(p => !string.IsNullOrEmpty(p.Description))
                    .WithMessage("Plot description cannot exceed 2000 characters");

                plot.RuleFor(p => p.Resolution)
                    .MaximumLength(2000)
                    .When(p => !string.IsNullOrEmpty(p.Resolution))
                    .WithMessage("Plot resolution cannot exceed 2000 characters");
            })
            .When(x => x.Plots != null);

        // Chapter suggestions validation
        RuleForEach(x => x.Chapters)
            .ChildRules(chapter =>
            {
                chapter.RuleFor(c => c.Title)
                    .NotEmpty()
                    .WithMessage("Chapter title is required")
                    .MaximumLength(200)
                    .WithMessage("Chapter title cannot exceed 200 characters");

                chapter.RuleFor(c => c.Summary)
                    .MaximumLength(2000)
                    .When(c => !string.IsNullOrEmpty(c.Summary))
                    .WithMessage("Chapter summary cannot exceed 2000 characters");

                chapter.RuleFor(c => c.Order)
                    .GreaterThan(0)
                    .WithMessage("Chapter order must be greater than 0");
            })
            .When(x => x.Chapters != null);
    }
}
