using AutorLLM.Domain.Interfaces;
using FluentValidation;

namespace AutorLLM.Application.Commands.Projects.CreateProject;

/// <summary>
/// Validator for CreateProjectCommand
/// </summary>
public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    private readonly IProjectRepository _projectRepository;

    public CreateProjectCommandValidator(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must be under 200 characters")
            .MustAsync(BeUniqueTitle)
                .WithMessage("A project with this title already exists");

        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("Author is required")
            .MaximumLength(100).WithMessage("Author must be under 100 characters");

        RuleFor(x => x.Synopsis)
            .MaximumLength(2000).WithMessage("Synopsis must be under 2000 characters");
    }

    private async Task<bool> BeUniqueTitle(string title, CancellationToken cancellationToken)
    {
        var existingProject = await _projectRepository.GetByTitleAsync(title, cancellationToken);
        return existingProject == null;
    }
}
