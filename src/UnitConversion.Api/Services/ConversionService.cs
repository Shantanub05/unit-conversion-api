using UnitConversion.Api.Models;
using UnitConversion.Api.Services.Interfaces;

namespace UnitConversion.Api.Services;

/// <summary>
/// Orchestrates unit conversions by delegating to the appropriate <see cref="IUnitConverter"/>
/// based on the requested source and target units.
/// </summary>
public sealed class ConversionService : IConversionService
{
    private readonly List<IUnitConverter> _converters;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConversionService"/> class.
    /// </summary>
    /// <param name="converters">The collection of unit converters registered via dependency injection.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="converters"/> is null.</exception>
    public ConversionService(IEnumerable<IUnitConverter> converters)
    {
        ArgumentNullException.ThrowIfNull(converters);
        _converters = converters.ToList();
    }

    /// <inheritdoc />
    /// <remarks>
    /// Iterates through all registered converters to find one that can handle the requested
    /// conversion. If no single converter supports both units, it checks whether the units
    /// belong to different categories to provide a meaningful error message.
    /// </remarks>
    public ConversionResult Convert(double value, string fromUnit, string toUnit)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fromUnit);
        ArgumentException.ThrowIfNullOrWhiteSpace(toUnit);

        // Find a converter that can handle both units
        foreach (var converter in _converters)
        {
            if (converter.CanConvert(fromUnit, toUnit))
            {
                var convertedValue = converter.Convert(value, fromUnit, toUnit);
                return new ConversionResult
                {
                    OriginalValue = value,
                    FromUnit = fromUnit,
                    ConvertedValue = convertedValue,
                    ToUnit = toUnit,
                    Category = converter.Category
                };
            }
        }

        // No converter found — determine whether this is a cross-category or unknown unit error
        string? fromCategory = FindCategoryForUnit(fromUnit);
        string? toCategory = FindCategoryForUnit(toUnit);

        if (fromCategory is not null && toCategory is not null)
        {
            throw new ArgumentException(
                $"Cannot convert between '{fromUnit}' ({fromCategory}) and '{toUnit}' ({toCategory}). " +
                $"Units belong to different categories.");
        }

        if (fromCategory is not null)
        {
            throw new ArgumentException(
                $"Unsupported unit: '{toUnit}'. The source unit '{fromUnit}' belongs to the '{fromCategory}' category.");
        }

        if (toCategory is not null)
        {
            throw new ArgumentException(
                $"Unsupported unit: '{fromUnit}'. The target unit '{toUnit}' belongs to the '{toCategory}' category.");
        }

        throw new ArgumentException(
            $"Unsupported unit(s): '{fromUnit}' and/or '{toUnit}'. No matching converter found.");
    }

    /// <inheritdoc />
    public IReadOnlyCollection<string> GetCategories()
    {
        return _converters
            .Select(c => c.Category)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(c => c, StringComparer.OrdinalIgnoreCase)
            .ToList()
            .AsReadOnly();
    }

    /// <inheritdoc />
    public IDictionary<string, IReadOnlyCollection<UnitInfo>> GetUnits(string? category = null)
    {
        if (category is not null)
        {
            var matchingConverter = _converters
                .FirstOrDefault(c => c.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

            if (matchingConverter is null)
            {
                throw new KeyNotFoundException($"Unknown category: '{category}'. Available categories: {string.Join(", ", GetCategories())}.");
            }

            return new Dictionary<string, IReadOnlyCollection<UnitInfo>>
            {
                [matchingConverter.Category] = matchingConverter.SupportedUnits
            };
        }

        // Return all units grouped by category
        var result = new Dictionary<string, IReadOnlyCollection<UnitInfo>>(StringComparer.OrdinalIgnoreCase);

        foreach (var converter in _converters)
        {
            result[converter.Category] = converter.SupportedUnits;
        }

        return result;
    }

    /// <summary>
    /// Searches all registered converters to find which category a unit belongs to.
    /// </summary>
    /// <param name="unit">The unit name to look up (case-insensitive).</param>
    /// <returns>The category name if the unit is found; otherwise, <c>null</c>.</returns>
    private string? FindCategoryForUnit(string unit)
    {
        foreach (var converter in _converters)
        {
            var match = converter.SupportedUnits
                .Any(u => u.Name.Equals(unit, StringComparison.OrdinalIgnoreCase) ||
                          u.Abbreviation.Equals(unit, StringComparison.OrdinalIgnoreCase));

            if (match)
            {
                return converter.Category;
            }
        }

        return null;
    }
}
