namespace UnitConversion.Api.Models;

/// <summary>
/// Represents the result of a unit conversion operation.
/// </summary>
public sealed record ConversionResult
{
    /// <summary>
    /// The original value that was converted.
    /// </summary>
    /// <example>100</example>
    public double OriginalValue { get; init; }

    /// <summary>
    /// The source unit the value was converted from.
    /// </summary>
    /// <example>meter</example>
    public string FromUnit { get; init; } = string.Empty;

    /// <summary>
    /// The converted numerical value.
    /// </summary>
    /// <example>328.084</example>
    public double ConvertedValue { get; init; }

    /// <summary>
    /// The target unit the value was converted to.
    /// </summary>
    /// <example>foot</example>
    public string ToUnit { get; init; } = string.Empty;

    /// <summary>
    /// The conversion category (e.g., "length", "temperature").
    /// </summary>
    /// <example>length</example>
    public string Category { get; init; } = string.Empty;
}
