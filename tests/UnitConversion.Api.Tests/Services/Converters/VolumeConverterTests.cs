using FluentAssertions;
using UnitConversion.Api.Services.Converters;

namespace UnitConversion.Api.Tests.Services.Converters;

public class VolumeConverterTests
{
    private readonly VolumeConverter _converter = new();

    [Fact]
    public void Convert_LitersToGallons()
    {
        // Arrange & Act
        var result = _converter.Convert(1, "liter", "gallon");

        // Assert
        result.Should().BeApproximately(0.2641720524, 0.001);
    }

    [Fact]
    public void Convert_GallonsToLiters()
    {
        // Arrange & Act
        var result = _converter.Convert(1, "gallon", "liter");

        // Assert
        result.Should().BeApproximately(3.785411784, 0.001);
    }

    [Fact]
    public void SupportedUnits_Contains6Units()
    {
        // Arrange & Act & Assert
        _converter.SupportedUnits.Should().HaveCount(6);
    }

    [Fact]
    public void Category_ReturnsVolume()
    {
        // Arrange & Act & Assert
        _converter.Category.Should().Be("volume");
    }
}
