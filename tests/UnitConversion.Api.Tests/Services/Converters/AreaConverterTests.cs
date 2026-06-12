using FluentAssertions;
using UnitConversion.Api.Services.Converters;

namespace UnitConversion.Api.Tests.Services.Converters;

public class AreaConverterTests
{
    private readonly AreaConverter _converter = new();

    [Fact]
    public void Convert_SquareMetersToSquareFeet()
    {
        // Arrange & Act
        var result = _converter.Convert(1, "square meter", "square foot");

        // Assert
        result.Should().BeApproximately(10.7639104167, 0.001);
    }

    [Fact]
    public void Convert_AcresToHectares()
    {
        // Arrange & Act
        var result = _converter.Convert(1, "acre", "hectare");

        // Assert
        result.Should().BeApproximately(0.4046856422, 0.001);
    }

    [Fact]
    public void SupportedUnits_Contains6Units()
    {
        // Arrange & Act & Assert
        _converter.SupportedUnits.Should().HaveCount(6);
    }

    [Fact]
    public void Category_ReturnsArea()
    {
        // Arrange & Act & Assert
        _converter.Category.Should().Be("area");
    }
}
