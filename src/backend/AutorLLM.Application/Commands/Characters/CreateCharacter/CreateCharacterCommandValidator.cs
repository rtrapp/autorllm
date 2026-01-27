using FluentValidation;

namespace AutorLLM.Application.Commands.Characters.CreateCharacter;

/// <summary>
/// Validator for CreateCharacterCommand
/// </summary>
public class CreateCharacterCommandValidator : AbstractValidator<CreateCharacterCommand>
{
    public CreateCharacterCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("ProjectId is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must be under 100 characters");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required")
            .MaximumLength(50).WithMessage("Role must be under 50 characters");

        RuleFor(x => x.Biography)
            .MaximumLength(5000).WithMessage("Biography must be under 5000 characters");
    }
}
