using FluentAssertions;
using UnitConversion.Api.Services.Converters;

namespace UnitConversion.Api.Tests.Services.Converters;

public class SpeedConverterTests
{
    private readonly SpeedConverter _converter = new();

    [Fact]
    public void Convert_KmhToMph()
    {
        // Arrange & Act
        var result = _converter.Convert(100, "kilometers per hour", "miles per hour");

        // Assert
        result.Should().BeApproximately(62.1371, 0.01);
    }

    [Fact]
    public void Convert_MpsToKmh()
    {
        // Arrange & Act
        var result = _converter.Convert(1, "meters per second", "kilometers per hour");

        // Assert
        result.Should().BeApproximately(3.6, 0.01);
    }

    [Fact]
    public void SupportedUnits_Contains4Units()
    {
        // Arrange & Act & Assert
        _converter.SupportedUnits.Should().HaveCount(4);
    }

    [Fact]
    public void Category_ReturnsSpeed()
    {
        // Arrange & Act & Assert
        _converter.Category.Should().Be("speed");
    }
}
