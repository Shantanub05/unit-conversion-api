using UnitConversion.Api.Models;
using UnitConversion.Api.Services.Interfaces;

namespace UnitConversion.Api.Services.Converters;

/// <summary>
/// Converts between various units of speed using base-unit normalization (meters per second).
/// </summary>
public class SpeedConverter : IUnitConverter
{
    private readonly Dictionary<string, (string Abbreviation, double Factor)> _units =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["meters per second"] = ("m/s", 1.0),
            ["kilometers per hour"] = ("km/h", 0.27777777777778),
            ["miles per hour"] = ("mph", 0.44704),
            ["knot"] = ("kn", 0.514444)
        };

    /// <inheritdoc />
    public string Category => "speed";

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
