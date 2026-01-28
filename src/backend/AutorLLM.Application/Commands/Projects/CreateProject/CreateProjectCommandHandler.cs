using AutorLLM.Application.Commands.Projects.CreateProject;
using AutorLLM.Domain.Aggregates.ProjectAggregate;
using AutorLLM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Commands.Projects.CreateProject;

/// <summary>
/// Handler for CreateProjectCommand
/// </summary>
public class CreateProjectCommandHandler 
    : IRequestHandler<CreateProjectCommand, CreateProjectResult>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateProjectCommandHandler> _logger;

    public CreateProjectCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateProjectCommandHandler> logger)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CreateProjectResult> Handle(
        CreateProjectCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating project: {Title} by {Author}",
            command.Title,
            command.Author);

        // Use domain entity factory method (encapsulated business logic)
        var project = Project.Create(
            command.Title,
            command.Author,
            command.Synopsis,
            command.Genre);

        // Persist through repository
        await _projectRepository.AddAsync(project, cancellationToken);

        // Commit transaction
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Project created with ID: {ProjectId}", project.Id);

        return new CreateProjectResult
        {
            ProjectId = project.Id,
            Success = true
        };
    }
}
