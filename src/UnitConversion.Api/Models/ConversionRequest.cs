namespace UnitConversion.Api.Models;

/// <summary>
/// Represents a request to convert a value from one unit to another.
/// </summary>
public sealed record ConversionRequest
{
    /// <summary>
    /// The numerical value to convert.
    /// </summary>
    /// <example>100</example>
    public double Value { get; init; }

    /// <summary>
    /// The source unit to convert from (case-insensitive).
    /// </summary>
    /// <example>meter</example>
    public string FromUnit { get; init; } = string.Empty;

    /// <summary>
    /// The target unit to convert to (case-insensitive).
    /// </summary>
    /// <example>foot</example>
    public string ToUnit { get; init; } = string.Empty;
}
