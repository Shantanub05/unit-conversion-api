using FluentAssertions;
using UnitConversion.Api.Services.Converters;

namespace UnitConversion.Api.Tests.Services.Converters;

public class WeightConverterTests
{
    private readonly WeightConverter _converter = new();

    [Fact]
    public void Convert_KilogramsToPounds_ReturnsCorrectValue()
    {
        // Arrange & Act
        var result = _converter.Convert(1, "kilogram", "pound");

        // Assert
        result.Should().BeApproximately(2.2046226218, 0.001);
    }

    [Fact]
    public void Convert_PoundsToKilograms()
    {
        // Arrange & Act
        var result = _converter.Convert(1, "pound", "kilogram");

        // Assert
        result.Should().BeApproximately(0.45359237, 0.001);
    }

    [Fact]
    public void Convert_GramsToOunces()
    {
        // Arrange & Act
        var result = _converter.Convert(28.349523125, "gram", "ounce");

        // Assert
        result.Should().BeApproximately(1, 0.001);
    }

    [Fact]
    public void Convert_TonsToKilograms()
    {
        // Arrange & Act
        var result = _converter.Convert(1, "ton", "kilogram");

        // Assert
        result.Should().Be(1000);
    }

    [Fact]
    public void Convert_SameUnit_ReturnsSameValue()
    {
        // Arrange & Act
        var result = _converter.Convert(5, "kilogram", "kilogram");

        // Assert
        result.Should().Be(5);
    }

    [Fact]
    public void Convert_ZeroValue_ReturnsZero()
    {
        // Arrange & Act
        var result = _converter.Convert(0, "kilogram", "pound");

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void CanConvert_ValidUnits_ReturnsTrue()
    {
        // Arrange & Act
        var result = _converter.CanConvert("kilogram", "pound");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Convert_InvalidUnit_ThrowsArgumentException()
    {
        // Arrange & Act
        var act = () => _converter.Convert(1, "kilogram", "bushel");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SupportedUnits_Contains7Units()
    {
        // Arrange & Act & Assert
        _converter.SupportedUnits.Should().HaveCount(7);
    }

    [Fact]
    public void Category_ReturnsWeight()
    {
        // Arrange & Act & Assert
        _converter.Category.Should().Be("weight");
    }
}
