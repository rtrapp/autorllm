using MediatR;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace AutorLLM.Application.Commands.Brainstorm;

/// <summary>
/// Command para gerar outline estruturado baseado no contexto acumulado do brainstorm.
/// </summary>
public record GenerateOutlineCommand : IRequest<GenerateOutlineResult>
{
    /// <summary>
    /// ID da sessão de brainstorm (para rastreamento).
    /// </summary>
    public string SessionId { get; init; } = string.Empty;

    /// <summary>
    /// Ideia inicial do livro.
    /// </summary>
    public string BookIdea { get; init; } = string.Empty;

    /// <summary>
    /// Título sugerido/definido pelo autor.
    /// </summary>
    public string? Title { get; init; }

    /// <summary>
    /// Nome do autor.
    /// </summary>
    public string? Author { get; init; }

    /// <summary>
    /// Gênero literário.
    /// </summary>
    public string? Genre { get; init; }

    /// <summary>
    /// Sinopse expandida durante o brainstorm.
    /// </summary>
    public string? Synopsis { get; init; }

    /// <summary>
    /// Tom/atmosfera da história.
    /// </summary>
    public string? Tone { get; init; }

    /// <summary>
    /// Público-alvo.
    /// </summary>
    public string? TargetAudience { get; init; }

    /// <summary>
    /// Personagens mencionados/definidos (formato livre ou estruturado).
    /// </summary>
    public List<CharacterSuggestion>? Characters { get; init; }

    /// <summary>
    /// Locais mencionados/definidos.
    /// </summary>
    public List<LocationSuggestion>? Locations { get; init; }

    /// <summary>
    /// Plots/tramas mencionados.
    /// </summary>
    public List<PlotSuggestion>? Plots { get; init; }

    /// <summary>
    /// Estrutura de capítulos sugerida.
    /// </summary>
    public List<ChapterSuggestion>? Chapters { get; init; }
}

/// <summary>
/// Sugestão de personagem extraída das respostas.
/// </summary>
public record CharacterSuggestion
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Role { get; init; }
    public string? Backstory { get; init; }
    public string? Appearance { get; init; }
    public string? Personality { get; init; }
}

/// <summary>
/// Sugestão de local extraída das respostas.
/// </summary>
public record LocationSuggestion
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Geography { get; init; }
    public string? Culture { get; init; }
    public string? Significance { get; init; }
}

/// <summary>
/// Sugestão de plot extraída das respostas.
/// </summary>
public record PlotSuggestion
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Type { get; init; }
    public string? Resolution { get; init; }
}

/// <summary>
/// Sugestão de capítulo extraída das respostas.
/// </summary>
public record ChapterSuggestion
{
    public string Title { get; init; } = string.Empty;
    public string? Summary { get; init; }
    public int Order { get; init; }
}

/// <summary>
/// Resultado do comando de geração de outline.
/// </summary>
public record GenerateOutlineResult
{
    public OutlineData Outline { get; init; } = null!;
    public List<string> ValidationErrors { get; init; } = new();
    public bool IsValid => ValidationErrors.Count == 0;
}

/// <summary>
/// Outline estruturado gerado pela LLM.
/// MAPEIA EXATAMENTE para as entidades de domínio.
/// </summary>
[Description("Complete book outline with project metadata, characters, locations, plots, and chapters")]
public record OutlineData
{
    // Project fields
    [JsonPropertyName("title")]
    [Description("Book title (max 200 characters)")]
    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("author")]
    [Description("Author name (max 100 characters)")]
    public string Author { get; init; } = string.Empty;

    [JsonPropertyName("synopsis")]
    [Description("Book synopsis (200-500 words)")]
    public string Synopsis { get; init; } = string.Empty;

    [JsonPropertyName("genre")]
    [Description("Literary genre (optional, max 50 characters)")]
    public string? Genre { get; init; }

    [JsonPropertyName("targetWordCount")]
    [Description("Target word count for the book (optional, default: 50000)")]
    public int? TargetWordCount { get; init; }

    // Child entities
    [JsonPropertyName("characters")]
    [Description("List of characters in the story (minimum 3 required)")]
    public List<CharacterData> Characters { get; init; } = new();

    [JsonPropertyName("locations")]
    [Description("List of locations in the story")]
    public List<LocationData> Locations { get; init; } = new();

    [JsonPropertyName("plots")]
    [Description("List of plots/story arcs (minimum 1 Main plot required)")]
    public List<PlotData> Plots { get; init; } = new();

    [JsonPropertyName("chapters")]
    [Description("List of chapters (5-12 chapters required)")]
    public List<ChapterData> Chapters { get; init; } = new();
}

[Description("Character information including role, backstory, appearance and personality")]
public record CharacterData
{
    [JsonPropertyName("name")]
    [Description("Character name (max 100 characters)")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    [Description("Brief character description (max 1000 characters)")]
    public string Description { get; init; } = string.Empty;

    [JsonPropertyName("role")]
    [Description("Character role: Protagonist, Antagonist, Supporting, or Minor")]
    public string Role { get; init; } = "Supporting";

    [JsonPropertyName("backstory")]
    [Description("Character backstory (optional, max 5000 characters)")]
    public string? Backstory { get; init; }

    [JsonPropertyName("appearance")]
    [Description("Physical appearance description (optional, max 2000 characters)")]
    public string? Appearance { get; init; }

    [JsonPropertyName("personality")]
    [Description("Personality traits and behavior (optional, max 2000 characters)")]
    public string? Personality { get; init; }
}

[Description("Location information including geography, culture and significance")]
public record LocationData
{
    [JsonPropertyName("name")]
    [Description("Location name (max 100 characters)")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    [Description("Location description (max 1000 characters)")]
    public string Description { get; init; } = string.Empty;

    [JsonPropertyName("geography")]
    [Description("Geographic details (optional, max 2000 characters)")]
    public string? Geography { get; init; }

    [JsonPropertyName("culture")]
    [Description("Cultural aspects (optional, max 2000 characters)")]
    public string? Culture { get; init; }

    [JsonPropertyName("significance")]
    [Description("Story significance (optional, max 1000 characters)")]
    public string? Significance { get; init; }
}

[Description("Plot/story arc information including type and resolution")]
public record PlotData
{
    [JsonPropertyName("title")]
    [Description("Plot title (max 200 characters)")]
    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    [Description("Plot description (max 2000 characters)")]
    public string Description { get; init; } = string.Empty;

    [JsonPropertyName("type")]
    [Description("Plot type: Main, Subplot, Character Arc, Romance, or Mystery")]
    public string Type { get; init; } = "Subplot";

    [JsonPropertyName("resolution")]
    [Description("How the plot resolves (optional, max 2000 characters)")]
    public string? Resolution { get; init; }
}

[Description("Chapter information with title, summary and order")]
public record ChapterData
{
    [JsonPropertyName("title")]
    [Description("Chapter title (max 200 characters)")]
    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("summary")]
    [Description("Chapter summary/synopsis (max 2000 characters)")]
    public string Summary { get; init; } = string.Empty;

    [JsonPropertyName("order")]
    [Description("Chapter order/number (1, 2, 3...)")]
    public int Order { get; init; }
}
