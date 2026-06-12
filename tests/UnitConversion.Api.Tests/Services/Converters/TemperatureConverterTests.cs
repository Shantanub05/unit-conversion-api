using FluentAssertions;
using UnitConversion.Api.Services.Converters;

namespace UnitConversion.Api.Tests.Services.Converters;

public class TemperatureConverterTests
{
    private readonly TemperatureConverter _converter = new();

    [Fact]
    public void Convert_CelsiusToFahrenheit_BoilingPoint()
    {
        // Arrange & Act
        var result = _converter.Convert(100, "celsius", "fahrenheit");

        // Assert
        result.Should().Be(212);
    }

    [Fact]
    public void Convert_CelsiusToFahrenheit_FreezingPoint()
    {
        // Arrange & Act
        var result = _converter.Convert(0, "celsius", "fahrenheit");

        // Assert
        result.Should().Be(32);
    }

    [Fact]
    public void Convert_FahrenheitToCelsius_BoilingPoint()
    {
        // Arrange & Act
        var result = _converter.Convert(212, "fahrenheit", "celsius");

        // Assert
        result.Should().Be(100);
    }

    [Fact]
    public void Convert_CelsiusToKelvin_AbsoluteZero()
    {
        // Arrange & Act
        var result = _converter.Convert(-273.15, "celsius", "kelvin");

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void Convert_KelvinToCelsius()
    {
        // Arrange & Act
        var result = _converter.Convert(373.15, "kelvin", "celsius");

        // Assert
        result.Should().Be(100);
    }

    [Fact]
    public void Convert_KelvinToFahrenheit()
    {
        // Arrange & Act
        var result = _converter.Convert(0, "kelvin", "fahrenheit");

        // Assert
        result.Should().BeApproximately(-459.67, 0.001);
    }

    [Fact]
    public void Convert_FahrenheitToKelvin()
    {
        // Arrange & Act
        var result = _converter.Convert(32, "fahrenheit", "kelvin");

        // Assert
        result.Should().BeApproximately(273.15, 0.001);
    }

    [Fact]
    public void Convert_SameUnit_ReturnsSameValue()
    {
        // Arrange & Act
        var result = _converter.Convert(100, "celsius", "celsius");

        // Assert
        result.Should().Be(100);
    }

    [Fact]
    public void Convert_NegativeTemperature()
    {
        // Arrange & Act
        var result = _converter.Convert(-40, "celsius", "fahrenheit");

        // Assert
        result.Should().Be(-40);
    }

    [Fact]
    public void CanConvert_ValidUnits_ReturnsTrue()
    {
        // Arrange & Act
        var result = _converter.CanConvert("celsius", "fahrenheit");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Convert_InvalidUnit_ThrowsArgumentException()
    {
        // Arrange & Act
        var act = () => _converter.Convert(1, "celsius", "rankine");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SupportedUnits_Contains3Units()
    {
        // Arrange & Act & Assert
        _converter.SupportedUnits.Should().HaveCount(3);
    }
}
