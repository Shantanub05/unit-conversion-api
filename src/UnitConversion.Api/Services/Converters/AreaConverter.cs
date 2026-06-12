using UnitConversion.Api.Models;
using UnitConversion.Api.Services.Interfaces;

namespace UnitConversion.Api.Services.Converters;

/// <summary>
/// Converts between various units of area using base-unit normalization (square meter).
/// </summary>
public class AreaConverter : IUnitConverter
{
    private readonly Dictionary<string, (string Abbreviation, double Factor)> _units =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["square meter"] = ("m²", 1.0),
            ["square foot"] = ("ft²", 0.09290304),
            ["acre"] = ("ac", 4046.8564224),
            ["hectare"] = ("ha", 10000.0),
            ["square kilometer"] = ("km²", 1000000.0),
            ["square mile"] = ("mi²", 2589988.110336)
        };

    /// <inheritdoc />
    public string Category => "area";

    /// <inheritdoc />
    public IReadOnlyCollection<UnitInfo> SupportedUnits =>
        _units.Select(u => new UnitInfo
        {
            Name = u.Key,
            Abbreviation = u.Value.Abbreviation,
            Category = Category
        }).ToList().AsReadOnly();

    /// <inheritdoc />
    public bool CanConvert(string fromUnit, string toUnit) =>
        _units.ContainsKey(fromUnit) && _units.ContainsKey(toUnit);

    /// <inheritdoc />
    public double Convert(double value, string fromUnit, string toUnit)
    {
        if (!_units.TryGetValue(fromUnit, out var from))
            throw new ArgumentException($"Unsupported source unit: '{fromUnit}'.", nameof(fromUnit));

        if (!_units.TryGetValue(toUnit, out var to))
            throw new ArgumentException($"Unsupported target unit: '{toUnit}'.", nameof(toUnit));

        var baseValue = value * from.Factor;
        var result = baseValue / to.Factor;
        return Math.Round(result, 10);
    }
}
