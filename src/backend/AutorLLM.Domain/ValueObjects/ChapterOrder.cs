using AutorLLM.Domain.Common;

namespace AutorLLM.Domain.ValueObjects;

/// <summary>
/// Represents the order/sequence position of a chapter.
/// Value object - immutable and defined by its value.
/// </summary>
public sealed class ChapterOrder : ValueObject
{
    public int Value { get; }

    private ChapterOrder(int value)
    {
        if (value <= 0)
            throw new ArgumentException("Chapter order must be greater than 0.", nameof(value));

        Value = value;
    }

    public static ChapterOrder Create(int value)
    {
        return new ChapterOrder(value);
    }

    public ChapterOrder Next()
    {
        return new ChapterOrder(Value + 1);
    }

    public ChapterOrder Previous()
    {
        if (Value == 1)
            throw new InvalidOperationException("Cannot get previous order of the first chapter.");

        return new ChapterOrder(Value - 1);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();

    public static implicit operator int(ChapterOrder order) => order.Value;
}
