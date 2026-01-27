using AutorLLM.Domain.Aggregates.ProjectAggregate;
using AutorLLM.Domain.Entities;

namespace AutorLLM.Domain.Interfaces;

/// <summary>
/// Repository interface for Project aggregate root
/// </summary>
public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Project> AddAsync(Project project, CancellationToken cancellationToken = default);
    Task UpdateAsync(Project project, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    
    // Child entity queries
    Task<Chapter?> GetChapterByIdAsync(Guid chapterId, CancellationToken cancellationToken = default);
}
