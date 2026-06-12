using UnitConversion.Api.Models;
using UnitConversion.Api.Services.Interfaces;

namespace UnitConversion.Api.Services.Converters;

/// <summary>
/// Converts between various units of volume using base-unit normalization (liter).
/// </summary>
public class VolumeConverter : IUnitConverter
{
    private readonly Dictionary<string, (string Abbreviation, double Factor)> _units =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["liter"] = ("L", 1.0),
            ["milliliter"] = ("mL", 0.001),
            ["gallon"] = ("gal", 3.785411784),
            ["cup"] = ("cup", 0.2365882365),
            ["cubic meter"] = ("m³", 1000.0),
            ["fluid ounce"] = ("fl oz", 0.0295735295625)
        };

    /// <inheritdoc />
    public string Category => "volume";

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
