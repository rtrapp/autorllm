using FluentValidation;

namespace AutorLLM.Application.Commands.LLM.StartBrainstorm;

public class StartBrainstormCommandValidator : AbstractValidator<StartBrainstormCommand>
{
    public StartBrainstormCommandValidator()
    {
        RuleFor(x => x.BookIdea)
            .NotEmpty().WithMessage("BookIdea is required")
            .MinimumLength(20).WithMessage("BookIdea must be at least 20 characters")
            .MaximumLength(5000).WithMessage("BookIdea must be under 5000 characters");
    }
}
