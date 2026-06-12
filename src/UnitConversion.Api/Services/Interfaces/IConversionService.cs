using UnitConversion.Api.Models;

namespace UnitConversion.Api.Services.Interfaces;

/// <summary>
/// Orchestrates unit conversions by delegating to the appropriate <see cref="IUnitConverter"/>.
/// </summary>
public interface IConversionService
{
    /// <summary>
    /// Converts a value from one unit to another.
    /// </summary>
    /// <param name="value">The numerical value to convert.</param>
    /// <param name="fromUnit">The source unit (case-insensitive).</param>
    /// <param name="toUnit">The target unit (case-insensitive).</param>
    /// <returns>A <see cref="ConversionResult"/> containing the converted value and metadata.</returns>
    /// <exception cref="ArgumentException">Thrown when the units are invalid or belong to different categories.</exception>
    ConversionResult Convert(double value, string fromUnit, string toUnit);

    /// <summary>
    /// Gets the names of all supported conversion categories.
    /// </summary>
    IReadOnlyCollection<string> GetCategories();

    /// <summary>
    /// Gets information about all supported units, optionally filtered by category.
    /// </summary>
    /// <param name="category">Optional category name to filter by (case-insensitive). Null returns all units.</param>
    /// <returns>A dictionary mapping category names to their supported units.</returns>
    /// <exception cref="ArgumentException">Thrown when the specified category does not exist.</exception>
    IDictionary<string, IReadOnlyCollection<UnitInfo>> GetUnits(string? category = null);
}
