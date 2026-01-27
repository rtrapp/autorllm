using FluentValidation;

namespace AutorLLM.Application.Commands.Projects.DeleteProject;

/// <summary>
/// Validator for DeleteProjectCommand
/// </summary>
public class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
{
    public DeleteProjectCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty().WithMessage("ProjectId is required");
    }
}
