using UnitConversion.Api.Models;
using UnitConversion.Api.Services.Interfaces;

namespace UnitConversion.Api.Services.Converters;

/// <summary>
/// Converts between various units of length using base-unit normalization (meter).
/// </summary>
public class LengthConverter : IUnitConverter
{
    private readonly Dictionary<string, (string Abbreviation, double Factor)> _units =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["meter"] = ("m", 1.0),
            ["kilometer"] = ("km", 1000.0),
            ["centimeter"] = ("cm", 0.01),
            ["millimeter"] = ("mm", 0.001),
            ["mile"] = ("mi", 1609.344),
            ["yard"] = ("yd", 0.9144),
            ["foot"] = ("ft", 0.3048),
            ["inch"] = ("in", 0.0254)
        };

    /// <inheritdoc />
    public string Category => "length";

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
