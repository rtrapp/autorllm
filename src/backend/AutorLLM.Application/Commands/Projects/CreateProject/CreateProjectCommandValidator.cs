using FluentValidation;

namespace AutorLLM.Application.Commands.Projects.CreateProject;

/// <summary>
/// Validator for CreateProjectCommand
/// </summary>
public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must be under 200 characters");

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("Author is required")
            .MaximumLength(100).WithMessage("Author must be under 100 characters");

        RuleFor(x => x.Synopsis)
            .MaximumLength(2000).WithMessage("Synopsis must be under 2000 characters");
    }
}
