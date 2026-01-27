using AutorLLM.Domain.Aggregates.ProjectAggregate;
using AutorLLM.Domain.Entities;
using AutorLLM.Domain.Interfaces;
using AutorLLM.Domain.ValueObjects;
using Dapper;
using Npgsql;
using System.Data;

namespace AutorLLM.Infrastructure.Data.Repositories;

/// <summary>
/// PostgreSQL implementation of IProjectRepository using Dapper
/// Manages Project aggregate and all its child entities (Characters, Chapters, Plots, Locations)
/// </summary>
public class ProjectRepository : IProjectRepository
{
    private readonly IDbConnection _connection;

    public ProjectRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string projectSql = @"
            SELECT id, title, author, synopsis, genre, target_word_count AS targetwordcount, 
                   current_word_count AS currentwordcount, created_at AS createdat, updated_at AS updatedat
            FROM projects
            WHERE id = @Id";

        var projectData = await _connection.QuerySingleOrDefaultAsync(
            new CommandDefinition(projectSql, new { Id = id }, cancellationToken: cancellationToken));

        if (projectData == null)
            return null;

        var project = MapToEntity(projectData);

        // Load Characters
        await LoadCharactersAsync(project, cancellationToken);

        // Load Locations
        await LoadLocationsAsync(project, cancellationToken);

        // Load Plots
        await LoadPlotsAsync(project, cancellationToken);

        // Load Chapters
        await LoadChaptersAsync(project, cancellationToken);

        return project;
    }

    public async Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT id, title, author, synopsis, genre, target_word_count AS targetwordcount,
                   current_word_count AS currentwordcount, created_at AS createdat, updated_at AS updatedat
            FROM projects
            ORDER BY created_at DESC";

        var results = await _connection.QueryAsync(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        var projects = new List<Project>();
        foreach (var row in results)
        {
            var project = MapToEntity(row);
            await LoadCharactersAsync(project, cancellationToken);
            await LoadLocationsAsync(project, cancellationToken);
            await LoadPlotsAsync(project, cancellationToken);
            await LoadChaptersAsync(project, cancellationToken);
            projects.Add(project);
        }

        return projects;
    }

    public async Task<Project> AddAsync(Project project, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO projects (id, title, author, synopsis, genre, target_word_count, current_word_count, created_at, updated_at)
            VALUES (@Id, @Title, @Author, @Synopsis, @Genre, @TargetWordCount, @CurrentWordCount, @CreatedAt, @UpdatedAt)";

        await _connection.ExecuteAsync(
            new CommandDefinition(sql, new
            {
                Id = project.Id,
                Title = project.Title,
                Author = project.Author,
                Synopsis = project.Synopsis,
                Genre = project.Genre,
                TargetWordCount = project.TargetWordCount,
                CurrentWordCount = project.CurrentWordCount,
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt
            }, cancellationToken: cancellationToken));

        // Save Characters
        await SaveCharactersAsync(project, cancellationToken);

        // Save Locations
        await SaveLocationsAsync(project, cancellationToken);

        // Save Plots
        await SavePlotsAsync(project, cancellationToken);

        // Save Chapters
        await SaveChaptersAsync(project, cancellationToken);

        return project;
    }

    public async Task UpdateAsync(Project project, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE projects
            SET title = @Title,
                author = @Author,
                synopsis = @Synopsis,
                genre = @Genre,
                target_word_count = @TargetWordCount,
                current_word_count = @CurrentWordCount,
                updated_at = @UpdatedAt
            WHERE id = @Id";

        await _connection.ExecuteAsync(
            new CommandDefinition(sql, new
            {
                Id = project.Id,
                Title = project.Title,
                Author = project.Author,
                Synopsis = project.Synopsis,
                Genre = project.Genre,
                TargetWordCount = project.TargetWordCount,
                CurrentWordCount = project.CurrentWordCount,
                UpdatedAt = project.UpdatedAt
            }, cancellationToken: cancellationToken));

        // Sync Characters (delete all and re-insert)
        await SyncCharactersAsync(project, cancellationToken);

        // Sync Locations (delete all and re-insert)
        await SyncLocationsAsync(project, cancellationToken);

        // Sync Plots (delete all and re-insert)
        await SyncPlotsAsync(project, cancellationToken);

        // Sync Chapters (delete all and re-insert)
        await SyncChaptersAsync(project, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // CASCADE DELETE will handle children (characters, chapters, etc.)
        const string sql = "DELETE FROM projects WHERE id = @Id";

        await _connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT EXISTS(SELECT 1 FROM projects WHERE id = @Id)";

        return await _connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<Chapter?> GetChapterByIdAsync(Guid chapterId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT id, project_id AS projectid, title, summary, content, ""order"",
                   word_count AS wordcount, created_at AS createdat, updated_at AS updatedat
            FROM chapters
            WHERE id = @ChapterId";

        var chapterData = await _connection.QuerySingleOrDefaultAsync(
            new CommandDefinition(sql, new { ChapterId = chapterId }, cancellationToken: cancellationToken));

        if (chapterData == null)
            return null;

        return MapToChapterEntity(chapterData);
    }

    #region Private Helper Methods

    private static Project MapToEntity(dynamic row)
    {
        return Project.Hydrate(
            id: row.id,
            title: row.title,
            author: row.author,
            synopsis: row.synopsis,
            genre: row.genre,
            targetWordCount: row.targetwordcount,
            currentWordCount: row.currentwordcount,
            createdAt: row.createdat,
            updatedAt: row.updatedat
        );
    }

    private async Task LoadCharactersAsync(Project project, CancellationToken cancellationToken)
    {
        const string sql = @"
            SELECT id, project_id AS projectid, name, description, role, 
                   backstory, appearance, personality,
                   created_at AS createdat, updated_at AS updatedat
            FROM characters
            WHERE project_id = @ProjectId
            ORDER BY created_at";

        var characters = await _connection.QueryAsync(
            new CommandDefinition(sql, new { ProjectId = project.Id }, cancellationToken: cancellationToken));

        foreach (var charData in characters)
        {
            var character = MapToCharacterEntity(charData);
            // Use internal hydration method (bypasses business rules)
            project.HydrateCharacter(character);
        }
    }

    private async Task SaveCharactersAsync(Project project, CancellationToken cancellationToken)
    {
        const string sql = @"
            INSERT INTO characters (id, project_id, name, description, role, backstory, appearance, personality, created_at, updated_at)
            VALUES (@Id, @ProjectId, @Name, @Description, @Role, @Backstory, @Appearance, @Personality, @CreatedAt, @UpdatedAt)";

        foreach (var character in project.Characters)
        {
            await _connection.ExecuteAsync(
                new CommandDefinition(sql, new
                {
                    Id = character.Id,
                    ProjectId = character.ProjectId,
                    Name = character.Name,
                    Description = character.Description,
                    Role = character.Role.ToString(),
                    Backstory = character.Backstory,
                    Appearance = character.Appearance,
                    Personality = character.Personality,
                    CreatedAt = character.CreatedAt,
                    UpdatedAt = character.UpdatedAt
                }, cancellationToken: cancellationToken));
        }
    }

    private async Task SyncCharactersAsync(Project project, CancellationToken cancellationToken)
    {
        // Delete all existing characters for this project
        const string deleteSql = "DELETE FROM characters WHERE project_id = @ProjectId";
        await _connection.ExecuteAsync(
            new CommandDefinition(deleteSql, new { ProjectId = project.Id }, cancellationToken: cancellationToken));

        // Re-insert all characters from the aggregate
        await SaveCharactersAsync(project, cancellationToken);
    }

    private static Character MapToCharacterEntity(dynamic row)
    {
        var role = CharacterRole.FromString((string)row.role);
        
        return Character.Hydrate(
            id: row.id,
            projectId: row.projectid,
            name: row.name,
            description: row.description ?? string.Empty,
            role: role,
            backstory: row.backstory,
            appearance: row.appearance,
            personality: row.personality,
            createdAt: row.createdat,
            updatedAt: row.updatedat
        );
    }

    private async Task LoadLocationsAsync(Project project, CancellationToken cancellationToken)
    {
        const string sql = @"
            SELECT id, project_id AS projectid, name, description, geography, 
                   culture, significance,
                   created_at AS createdat, updated_at AS updatedat
            FROM locations
            WHERE project_id = @ProjectId
            ORDER BY created_at";

        var locations = await _connection.QueryAsync(
            new CommandDefinition(sql, new { ProjectId = project.Id }, cancellationToken: cancellationToken));

        foreach (var locationData in locations)
        {
            var location = MapToLocationEntity(locationData);
            // Use internal hydration method (bypasses business rules)
            project.HydrateLocation(location);
        }
    }

    private async Task SaveLocationsAsync(Project project, CancellationToken cancellationToken)
    {
        const string sql = @"
            INSERT INTO locations (id, project_id, name, description, geography, culture, significance, created_at, updated_at)
            VALUES (@Id, @ProjectId, @Name, @Description, @Geography, @Culture, @Significance, @CreatedAt, @UpdatedAt)";

        foreach (var location in project.Locations)
        {
            await _connection.ExecuteAsync(
                new CommandDefinition(sql, new
                {
                    Id = location.Id,
                    ProjectId = location.ProjectId,
                    Name = location.Name,
                    Description = location.Description,
                    Geography = location.Geography,
                    Culture = location.Culture,
                    Significance = location.Significance,
                    CreatedAt = location.CreatedAt,
                    UpdatedAt = location.UpdatedAt
                }, cancellationToken: cancellationToken));
        }
    }

    private async Task SyncLocationsAsync(Project project, CancellationToken cancellationToken)
    {
        // Delete all existing locations for this project
        const string deleteSql = "DELETE FROM locations WHERE project_id = @ProjectId";
        await _connection.ExecuteAsync(
            new CommandDefinition(deleteSql, new { ProjectId = project.Id }, cancellationToken: cancellationToken));

        // Re-insert all locations from the aggregate
        await SaveLocationsAsync(project, cancellationToken);
    }

    private static Location MapToLocationEntity(dynamic row)
    {
        return Location.Hydrate(
            id: row.id,
            projectId: row.projectid,
            name: row.name,
            description: row.description ?? string.Empty,
            geography: row.geography,
            culture: row.culture,
            significance: row.significance,
            createdAt: row.createdat,
            updatedAt: row.updatedat
        );
    }

    private async Task LoadPlotsAsync(Project project, CancellationToken cancellationToken)
    {
        const string sql = @"
            SELECT id, project_id AS projectid, title, description, type, 
                   resolution, is_active AS isactive,
                   created_at AS createdat, updated_at AS updatedat
            FROM plots
            WHERE project_id = @ProjectId
            ORDER BY created_at";

        var plots = await _connection.QueryAsync(
            new CommandDefinition(sql, new { ProjectId = project.Id }, cancellationToken: cancellationToken));

        foreach (var plotData in plots)
        {
            var plot = MapToPlotEntity(plotData);
            // Use internal hydration method (bypasses business rules)
            project.HydratePlot(plot);
        }
    }

    private async Task SavePlotsAsync(Project project, CancellationToken cancellationToken)
    {
        const string sql = @"
            INSERT INTO plots (id, project_id, title, description, type, resolution, is_active, created_at, updated_at)
            VALUES (@Id, @ProjectId, @Title, @Description, @Type, @Resolution, @IsActive, @CreatedAt, @UpdatedAt)";

        foreach (var plot in project.Plots)
        {
            await _connection.ExecuteAsync(
                new CommandDefinition(sql, new
                {
                    Id = plot.Id,
                    ProjectId = plot.ProjectId,
                    Title = plot.Title,
                    Description = plot.Description,
                    Type = plot.Type.ToString(),
                    Resolution = plot.Resolution,
                    IsActive = plot.IsActive,
                    CreatedAt = plot.CreatedAt,
                    UpdatedAt = plot.UpdatedAt
                }, cancellationToken: cancellationToken));
        }
    }

    private async Task SyncPlotsAsync(Project project, CancellationToken cancellationToken)
    {
        // Delete all existing plots for this project
        const string deleteSql = "DELETE FROM plots WHERE project_id = @ProjectId";
        await _connection.ExecuteAsync(
            new CommandDefinition(deleteSql, new { ProjectId = project.Id }, cancellationToken: cancellationToken));

        // Re-insert all plots from the aggregate
        await SavePlotsAsync(project, cancellationToken);
    }

    private static Plot MapToPlotEntity(dynamic row)
    {
        var plotType = PlotType.Create((string)row.type);
        
        return Plot.Hydrate(
            id: row.id,
            projectId: row.projectid,
            title: row.title,
            description: row.description ?? string.Empty,
            type: plotType,
            resolution: row.resolution,
            isActive: row.isactive,
            createdAt: row.createdat,
            updatedAt: row.updatedat
        );
    }

    private async Task LoadChaptersAsync(Project project, CancellationToken cancellationToken)
    {
        const string sql = @"
            SELECT id, project_id AS projectid, title, summary, content, ""order"",
                   word_count AS wordcount, created_at AS createdat, updated_at AS updatedat
            FROM chapters
            WHERE project_id = @ProjectId
            ORDER BY ""order""";

        var chapters = await _connection.QueryAsync(
            new CommandDefinition(sql, new { ProjectId = project.Id }, cancellationToken: cancellationToken));

        foreach (var chapterData in chapters)
        {
            var chapter = MapToChapterEntity(chapterData);
            project.HydrateChapter(chapter);
        }
    }

    private async Task SaveChaptersAsync(Project project, CancellationToken cancellationToken)
    {
        const string sql = @"
            INSERT INTO chapters (id, project_id, title, summary, content, ""order"", word_count, created_at, updated_at)
            VALUES (@Id, @ProjectId, @Title, @Summary, @Content, @Order, @WordCount, @CreatedAt, @UpdatedAt)";

        foreach (var chapter in project.Chapters)
        {
            await _connection.ExecuteAsync(
                new CommandDefinition(sql, new
                {
                    Id = chapter.Id,
                    ProjectId = chapter.ProjectId,
                    Title = chapter.Title,
                    Summary = chapter.Summary,
                    Content = chapter.Content,
                    Order = chapter.Order.Value,
                    WordCount = chapter.WordCount,
                    CreatedAt = chapter.CreatedAt,
                    UpdatedAt = chapter.UpdatedAt
                }, cancellationToken: cancellationToken));
        }
    }

    private async Task SyncChaptersAsync(Project project, CancellationToken cancellationToken)
    {
        // Delete all existing chapters for this project
        const string deleteSql = "DELETE FROM chapters WHERE project_id = @ProjectId";
        await _connection.ExecuteAsync(
            new CommandDefinition(deleteSql, new { ProjectId = project.Id }, cancellationToken: cancellationToken));

        // Re-insert all chapters from the aggregate
        await SaveChaptersAsync(project, cancellationToken);
    }

    private static Chapter MapToChapterEntity(dynamic row)
    {
        return Chapter.Hydrate(
            id: row.id,
            projectId: row.projectid,
            title: row.title,
            summary: row.summary ?? string.Empty,
            content: row.content ?? string.Empty,
            order: (int)row.order,
            wordCount: row.wordcount,
            createdAt: row.createdat,
            updatedAt: row.updatedat
        );
    }

    #endregion
}
