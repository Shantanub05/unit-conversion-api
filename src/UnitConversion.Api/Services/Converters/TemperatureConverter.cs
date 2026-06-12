using UnitConversion.Api.Models;
using UnitConversion.Api.Services.Interfaces;

namespace UnitConversion.Api.Services.Converters;

/// <summary>
/// Converts between temperature units using formula-based conversion.
/// Temperature conversions are non-linear, so base-unit normalization is not used.
/// </summary>
public class TemperatureConverter : IUnitConverter
{
    private readonly Dictionary<string, string> _units =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["celsius"] = "°C",
            ["fahrenheit"] = "°F",
            ["kelvin"] = "K"
        };

    /// <inheritdoc />
    public string Category => "temperature";

    /// <inheritdoc />
    public IReadOnlyCollection<UnitInfo> SupportedUnits =>
        _units.Select(u => new UnitInfo
        {
            Name = u.Key,
            Abbreviation = u.Value,
            Category = Category
        }).ToList().AsReadOnly();

    /// <inheritdoc />
    public bool CanConvert(string fromUnit, string toUnit) =>
        _units.ContainsKey(fromUnit) && _units.ContainsKey(toUnit);

    /// <inheritdoc />
    public double Convert(double value, string fromUnit, string toUnit)
    {
        if (!_units.ContainsKey(fromUnit))
            throw new ArgumentException($"Unsupported source unit: '{fromUnit}'.", nameof(fromUnit));

        if (!_units.ContainsKey(toUnit))
            throw new ArgumentException($"Unsupported target unit: '{toUnit}'.", nameof(toUnit));

        var from = fromUnit.ToLowerInvariant();
        var to = toUnit.ToLowerInvariant();

        if (from == to)
            return value;

        var result = (from, to) switch
        {
            ("celsius", "fahrenheit") => value * 9.0 / 5.0 + 32.0,
            ("celsius", "kelvin") => value + 273.15,
            ("fahrenheit", "celsius") => (value - 32.0) * 5.0 / 9.0,
            ("fahrenheit", "kelvin") => (value - 32.0) * 5.0 / 9.0 + 273.15,
            ("kelvin", "celsius") => value - 273.15,
            ("kelvin", "fahrenheit") => (value - 273.15) * 9.0 / 5.0 + 32.0,
            _ => throw new ArgumentException($"Unsupported conversion: '{fromUnit}' to '{toUnit}'.")
        };

        return Math.Round(result, 10);
    }
}
