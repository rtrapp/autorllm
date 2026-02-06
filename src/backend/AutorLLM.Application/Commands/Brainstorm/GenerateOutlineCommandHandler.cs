using System.Text.Json;
using AutorLLM.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AutorLLM.Application.Commands.Brainstorm;

/// <summary>
/// Handler para gerar outline estruturado usando Microsoft Agent Framework.
/// </summary>
public class GenerateOutlineCommandHandler : IRequestHandler<GenerateOutlineCommand, GenerateOutlineResult>
{
    private readonly IAgentService _agentService;
    private readonly AutorLLM.Application.AgentDefinitions.BrainstormAgentDefinition _brainstormAgent;
    private readonly ILogger<GenerateOutlineCommandHandler> _logger;

    public GenerateOutlineCommandHandler(
        IAgentService agentService,
        AutorLLM.Application.AgentDefinitions.BrainstormAgentDefinition brainstormAgent,
        ILogger<GenerateOutlineCommandHandler> logger)
    {
        _agentService = agentService;
        _brainstormAgent = brainstormAgent;
        _logger = logger;
    }

    public async Task<GenerateOutlineResult> Handle(
        GenerateOutlineCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating outline for session {SessionId}", request.SessionId);

        try
        {
            // Construir prompt com contexto acumulado
            var prompt = BuildPrompt(request);

            _logger.LogDebug("Prompt length: {Length} characters", prompt.Length);

            // Usar Agent Framework para gerar outline estruturado
            var outline = await _agentService.CompleteStructuredAsync<OutlineData>(_brainstormAgent, prompt, cancellationToken);
            _logger.LogDebug("Structured output received successfully");

            // Validar outline gerado
            var validationErrors = ValidateOutline(outline);

            if (validationErrors.Count > 0)
            {
                _logger.LogWarning(
                    "Outline validation failed with {Count} errors: {Errors}",
                    validationErrors.Count,
                    string.Join(", ", validationErrors)
                );
            }
            else
            {
                _logger.LogInformation(
                    "Outline generated successfully: {Title} with {CharCount} characters, {LocCount} locations, {PlotCount} plots, {ChapterCount} chapters",
                    outline.Title,
                    outline.Characters.Count,
                    outline.Locations.Count,
                    outline.Plots.Count,
                    outline.Chapters.Count
                );
            }

            return new GenerateOutlineResult
            {
                Outline = outline,
                ValidationErrors = validationErrors
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate outline for session {SessionId}", request.SessionId);
            
            // Retornar erro estruturado
            return new GenerateOutlineResult
            {
                Outline = new OutlineData
                {
                    Title = request.Title ?? "Untitled",
                    Author = request.Author ?? "Unknown Author",
                    Synopsis = request.Synopsis ?? request.BookIdea
                },
                ValidationErrors = new List<string> { $"Failed to generate outline: {ex.Message}" }
            };
        }
    }

    /// <summary>
    /// Constrói prompt para a LLM gerar outline estruturado.
    /// O Microsoft Agent Framework automaticamente aplica o JSON schema do tipo OutlineData.
    /// </summary>
    private string BuildPrompt(GenerateOutlineCommand request)
    {
        var contextParts = new List<string>();

        // Adicionar informações do contexto
        contextParts.Add($"**Ideia Inicial:** {request.BookIdea}");

        if (!string.IsNullOrEmpty(request.Title))
            contextParts.Add($"**Título Sugerido:** {request.Title}");

        if (!string.IsNullOrEmpty(request.Author))
            contextParts.Add($"**Autor:** {request.Author}");

        if (!string.IsNullOrEmpty(request.Genre))
            contextParts.Add($"**Gênero:** {request.Genre}");

        if (!string.IsNullOrEmpty(request.Tone))
            contextParts.Add($"**Tom:** {request.Tone}");

        if (!string.IsNullOrEmpty(request.Synopsis))
            contextParts.Add($"**Sinopse Expandida:** {request.Synopsis}");

        if (request.Characters?.Count > 0)
        {
            var charList = string.Join("\n", request.Characters.Select(c =>
                $"- {c.Name}{(string.IsNullOrEmpty(c.Description) ? "" : $": {c.Description}")}"));
            contextParts.Add($"**Personagens Mencionados:**\n{charList}");
        }

        if (request.Locations?.Count > 0)
        {
            var locList = string.Join("\n", request.Locations.Select(l =>
                $"- {l.Name}{(string.IsNullOrEmpty(l.Description) ? "" : $": {l.Description}")}"));
            contextParts.Add($"**Locais Mencionados:**\n{locList}");
        }

        if (request.Plots?.Count > 0)
        {
            var plotList = string.Join("\n", request.Plots.Select(p =>
                $"- {p.Title}{(string.IsNullOrEmpty(p.Description) ? "" : $": {p.Description}")}"));
            contextParts.Add($"**Tramas/Conflitos Mencionados:**\n{plotList}");
        }

        var context = string.Join("\n\n", contextParts);

        return @"Você é um assistente de escrita criativa especializado em estruturar histórias.

O autor forneceu as seguintes informações durante o brainstorm:

" + context + @"

**SUA TAREFA:**
Gere um outline estruturado completo para este livro baseado nas informações acima.

**REGRAS OBRIGATÓRIAS:**
1. Sinopse deve ter entre 200 e 500 palavras
2. Mínimo de 5 capítulos, máximo de 12
3. Mínimo de 3 personagens com roles: ""Protagonist"", ""Antagonist"", ""Supporting"", ou ""Minor""
4. Pelo menos 1 plot do tipo ""Main""
5. Todos os campos obrigatórios devem estar preenchidos
6. Use as informações fornecidas pelo autor como base, expandindo e complementando quando necessário
7. Mantenha consistência com o gênero literário e tom mencionados

Crie um outline criativo, coerente e bem estruturado que possa servir como base sólida para o desenvolvimento do livro.";
    }

    /// <summary>
    /// Tenta extrair e parsear JSON de OutlineData de uma resposta de texto livre da LLM.
    /// </summary>
    private OutlineData? TryParseOutlineFromResponse(string response)
    {
        try
        {
            // Tentar encontrar JSON entre ``` ou diretamente
            var jsonStart = response.IndexOf('{');
            var jsonEnd = response.LastIndexOf('}');

            if (jsonStart == -1 || jsonEnd == -1 || jsonEnd <= jsonStart)
            {
                _logger.LogWarning("No JSON braces found in response");
                return null;
            }

            var jsonText = response.Substring(jsonStart, jsonEnd - jsonStart + 1);
            _logger.LogDebug("Extracted JSON candidate (length: {Length})", jsonText.Length);

            // Parsear JSON
            var outline = JsonSerializer.Deserialize<OutlineData>(jsonText, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            });

            return outline;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to parse JSON from response");
            return null;
        }
    }

    /// <summary>
    /// Valida o outline gerado contra as regras de negócio.
    /// </summary>
    private List<string> ValidateOutline(OutlineData outline)
    {
        var errors = new List<string>();

        // Project validations
        if (string.IsNullOrWhiteSpace(outline.Title))
            errors.Add("Title is required");
        else if (outline.Title.Length > 200)
            errors.Add("Title cannot exceed 200 characters");

        if (string.IsNullOrWhiteSpace(outline.Author))
            errors.Add("Author is required");
        else if (outline.Author.Length > 100)
            errors.Add("Author cannot exceed 100 characters");

        if (string.IsNullOrWhiteSpace(outline.Synopsis))
            errors.Add("Synopsis is required");
        else
        {
            var wordCount = outline.Synopsis.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
            if (wordCount < 50) // ~200 palavras mínimo
                errors.Add("Synopsis should have at least 200 words");
            if (outline.Synopsis.Length > 5000)
                errors.Add("Synopsis cannot exceed 5000 characters");
        }

        // Characters validation
        if (outline.Characters.Count == 0)
            errors.Add("At least 1 character is required");

        // Plots validation
        if (outline.Plots.Count == 0)
            errors.Add("At least 1 plot is required");

        var hasMainPlot = outline.Plots.Any(p => p.Type == "Main");
        if (!hasMainPlot)
            errors.Add("At least 1 Main plot is required");

        // Chapters validation
        if (outline.Chapters.Count < 3)
            errors.Add("At least 3 chapters are required (got " + outline.Chapters.Count + ")");
        if (outline.Chapters.Count > 12)
            errors.Add("Maximum 12 chapters allowed for initial outline (got " + outline.Chapters.Count + ")");

        return errors;
    }
}
