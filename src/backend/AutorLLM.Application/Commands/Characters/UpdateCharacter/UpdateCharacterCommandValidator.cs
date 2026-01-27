using FluentValidation;

namespace AutorLLM.Application.Commands.Characters.UpdateCharacter;

/// <summary>
/// Validator for UpdateCharacterCommand
/// </summary>
public class UpdateCharacterCommandValidator : AbstractValidator<UpdateCharacterCommand>
{
    public UpdateCharacterCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("ProjectId is required.");

        RuleFor(x => x.CharacterId)
            .NotEmpty()
            .WithMessage("CharacterId is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Character name is required.")
            .MaximumLength(100)
            .WithMessage("Character name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Character description cannot exceed 1000 characters.");

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("Character role is required.")
            .Must(role => role == "Protagonist" || role == "Antagonist" || role == "Supporting")
            .WithMessage("Role must be one of: Protagonist, Antagonist, Supporting.");

        RuleFor(x => x.Backstory)
            .MaximumLength(5000)
            .When(x => x.Backstory != null)
            .WithMessage("Backstory cannot exceed 5000 characters.");

        RuleFor(x => x.Appearance)
            .MaximumLength(2000)
            .When(x => x.Appearance != null)
            .WithMessage("Appearance cannot exceed 2000 characters.");

        RuleFor(x => x.Personality)
            .MaximumLength(2000)
            .When(x => x.Personality != null)
            .WithMessage("Personality cannot exceed 2000 characters.");
    }
}
