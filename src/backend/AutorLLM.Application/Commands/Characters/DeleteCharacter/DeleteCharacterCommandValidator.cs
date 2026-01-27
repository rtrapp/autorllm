using FluentValidation;

namespace AutorLLM.Application.Commands.Characters.DeleteCharacter;

/// <summary>
/// Validator for DeleteCharacterCommand
/// </summary>
public class DeleteCharacterCommandValidator : AbstractValidator<DeleteCharacterCommand>
{
    public DeleteCharacterCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("ProjectId is required");

        RuleFor(x => x.CharacterId)
            .NotEmpty().WithMessage("CharacterId is required");
    }
}
