using FluentAssertions;
using UnitConversion.Api.Services.Converters;

namespace UnitConversion.Api.Tests.Services.Converters;

public class LengthConverterTests
{
    private readonly LengthConverter _converter = new();

    [Fact]
    public void Convert_MetersToFeet_ReturnsCorrectValue()
    {
        // Arrange & Act
        var result = _converter.Convert(100, "meter", "foot");

        // Assert
        result.Should().BeApproximately(328.0839895013, 0.001);
    }

    [Fact]
    public void Convert_KilometersToMiles_ReturnsCorrectValue()
    {
        // Arrange & Act
        var result = _converter.Convert(1, "kilometer", "mile");

        // Assert
        result.Should().BeApproximately(0.6213711922, 0.001);
    }

    [Fact]
    public void Convert_InchesToCentimeters_ReturnsCorrectValue()
    {
        // Arrange & Act
        var result = _converter.Convert(1, "inch", "centimeter");

        // Assert
        result.Should().Be(2.54);
    }

    [Fact]
    public void Convert_SameUnit_ReturnsSameValue()
    {
        // Arrange & Act
        var result = _converter.Convert(50, "meter", "meter");

        // Assert
        result.Should().Be(50);
    }

    [Fact]
    public void Convert_ZeroValue_ReturnsZero()
    {
        // Arrange & Act
        var result = _converter.Convert(0, "meter", "foot");

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void Convert_NegativeValue_ReturnsNegative()
    {
        // Arrange & Act
        var result = _converter.Convert(-10, "meter", "foot");

        // Assert
        result.Should().BeNegative();
    }

    [Fact]
    public void CanConvert_ValidUnits_ReturnsTrue()
    {
        // Arrange & Act
        var result = _converter.CanConvert("meter", "foot");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanConvert_InvalidUnit_ReturnsFalse()
    {
        // Arrange & Act
        var result = _converter.CanConvert("meter", "lightyear");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Convert_InvalidUnit_ThrowsArgumentException()
    {
        // Arrange & Act
        var act = () => _converter.Convert(1, "meter", "lightyear");

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Category_ReturnsLength()
    {
        // Arrange & Act & Assert
        _converter.Category.Should().Be("length");
    }

    [Fact]
    public void SupportedUnits_Contains8Units()
    {
        // Arrange & Act & Assert
        _converter.SupportedUnits.Should().HaveCount(8);
    }

    [Theory]
    [InlineData("METER", "FOOT")]
    [InlineData("Meter", "Foot")]
    public void Convert_CaseInsensitive(string fromUnit, string toUnit)
    {
        // Arrange & Act
        var result = _converter.Convert(100, fromUnit, toUnit);

        // Assert
        result.Should().BeApproximately(328.0839895013, 0.001);
    }
}
