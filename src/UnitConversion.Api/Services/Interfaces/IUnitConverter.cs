using UnitConversion.Api.Models;

namespace UnitConversion.Api.Services.Interfaces;

/// <summary>
/// Defines a converter for a specific category of unit measurements.
/// Each implementation handles conversions within a single category (e.g., length, temperature).
/// </summary>
public interface IUnitConverter
{
    /// <summary>
    /// The name of the conversion category (e.g., "length", "temperature").
    /// </summary>
    string Category { get; }

    /// <summary>
    /// Gets information about all units supported by this converter.
    /// </summary>
    IReadOnlyCollection<UnitInfo> SupportedUnits { get; }

    /// <summary>
    /// Determines whether this converter can handle a conversion between the specified units.
    /// </summary>
    /// <param name="fromUnit">The source unit name (case-insensitive).</param>
    /// <param name="toUnit">The target unit name (case-insensitive).</param>
    /// <returns>True if both units are supported by this converter; otherwise, false.</returns>
    bool CanConvert(string fromUnit, string toUnit);

    /// <summary>
    /// Converts a numerical value from one unit to another within this category.
    /// </summary>
    /// <param name="value">The numerical value to convert.</param>
    /// <param name="fromUnit">The source unit name (case-insensitive).</param>
    /// <param name="toUnit">The target unit name (case-insensitive).</param>
    /// <returns>The converted numerical value.</returns>
    /// <exception cref="ArgumentException">Thrown when an unsupported unit is specified.</exception>
    double Convert(double value, string fromUnit, string toUnit);
}
