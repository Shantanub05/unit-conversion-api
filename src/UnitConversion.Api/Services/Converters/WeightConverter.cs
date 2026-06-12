using UnitConversion.Api.Models;
using UnitConversion.Api.Services.Interfaces;

namespace UnitConversion.Api.Services.Converters;

/// <summary>
/// Converts between various units of weight using base-unit normalization (kilogram).
/// </summary>
public class WeightConverter : IUnitConverter
{
    private readonly Dictionary<string, (string Abbreviation, double Factor)> _units =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["kilogram"] = ("kg", 1.0),
            ["gram"] = ("g", 0.001),
            ["milligram"] = ("mg", 0.000001),
            ["pound"] = ("lb", 0.45359237),
            ["ounce"] = ("oz", 0.028349523125),
            ["ton"] = ("t", 1000.0),
            ["stone"] = ("st", 6.35029318)
        };

    /// <inheritdoc />
    public string Category => "weight";

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
