namespace UnitConversion.Api.Models;

/// <summary>
/// Represents information about a supported unit of measurement.
/// </summary>
public sealed record UnitInfo
{
    /// <summary>
    /// The canonical name of the unit (lowercase).
    /// </summary>
    /// <example>meter</example>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// The standard abbreviation for the unit.
    /// </summary>
    /// <example>m</example>
    public string Abbreviation { get; init; } = string.Empty;

    /// <summary>
    /// The conversion category this unit belongs to.
    /// </summary>
    /// <example>length</example>
    public string Category { get; init; } = string.Empty;
}
