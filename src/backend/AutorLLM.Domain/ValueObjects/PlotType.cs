using AutorLLM.Domain.Common;

namespace AutorLLM.Domain.ValueObjects;

/// <summary>
/// Represents the type/category of a plot line.
/// Value object - immutable and defined by its value.
/// </summary>
public sealed class PlotType : ValueObject
{
    public string Value { get; }

    // Predefined common plot types
    public static readonly PlotType Main = new("Main");
    public static readonly PlotType Subplot = new("Subplot");
    public static readonly PlotType Character = new("Character Arc");
    public static readonly PlotType Romance = new("Romance");
    public static readonly PlotType Mystery = new("Mystery");

    private PlotType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Plot type cannot be empty.", nameof(value));

        if (value.Length > 50)
            throw new ArgumentException("Plot type cannot exceed 50 characters.", nameof(value));

        Value = value.Trim();
    }

    public static PlotType Create(string value)
    {
        return new PlotType(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
