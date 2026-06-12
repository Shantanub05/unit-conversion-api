using FluentAssertions;
using UnitConversion.Api.Services;
using UnitConversion.Api.Services.Converters;
using UnitConversion.Api.Services.Interfaces;

namespace UnitConversion.Api.Tests.Services;

public class ConversionServiceTests
{
    private readonly IConversionService _conversionService;

    public ConversionServiceTests()
    {
        var converters = new List<IUnitConverter>
        {
            new LengthConverter(),
            new TemperatureConverter(),
            new WeightConverter(),
            new AreaConverter(),
            new VolumeConverter(),
            new SpeedConverter()
        };
        _conversionService = new ConversionService(converters);
    }

    [Fact]
    public void Convert_ValidConversion_ReturnsCorrectResult()
    {
        var result = _conversionService.Convert(100, "meter", "foot");

        result.OriginalValue.Should().Be(100);
        result.FromUnit.Should().Be("meter");
        result.ToUnit.Should().Be("foot");
        result.Category.Should().Be("length");
        result.ConvertedValue.Should().BeApproximately(328.0839895013, 0.001);
    }

    [Fact]
    public void Convert_CrossCategory_ThrowsArgumentException()
    {
        var action = () => _conversionService.Convert(100, "meter", "celsius");

        action.Should().Throw<ArgumentException>()
            .WithMessage("*Cannot convert between 'meter' (length) and 'celsius' (temperature)*");
    }

    [Fact]
    public void Convert_UnknownUnit_ThrowsArgumentException()
    {
        var action = () => _conversionService.Convert(100, "nonexistent", "alsonotreal");

        action.Should().Throw<ArgumentException>()
            .WithMessage("*Unsupported unit(s)*");
    }

    [Fact]
    public void GetCategories_ReturnsAllCategories()
    {
        var categories = _conversionService.GetCategories();

        categories.Should().HaveCount(6);
        categories.Should().Contain(new[] { "length", "temperature", "weight", "area", "volume", "speed" });
    }

    [Fact]
    public void GetCategories_ReturnsSortedCategories()
    {
        var categories = _conversionService.GetCategories().ToList();

        categories.Should().BeInAscendingOrder();
    }

    [Fact]
    public void GetUnits_NoFilter_ReturnsAllCategories()
    {
        var units = _conversionService.GetUnits();

        units.Keys.Should().HaveCount(6);
        units["length"].Should().NotBeEmpty();
    }

    [Fact]
    public void GetUnits_WithCategory_ReturnsFilteredUnits()
    {
        var units = _conversionService.GetUnits("length");

        units.Keys.Should().HaveCount(1);
        units.ContainsKey("length").Should().BeTrue();
    }

    [Fact]
    public void GetUnits_UnknownCategory_ThrowsKeyNotFoundException()
    {
        var action = () => _conversionService.GetUnits("nonexistent");

        action.Should().Throw<KeyNotFoundException>()
            .WithMessage("*Unknown category: 'nonexistent'*");
    }

    [Fact]
    public void Convert_CaseInsensitive_Works()
    {
        var result = _conversionService.Convert(100, "METER", "FOOT");

        result.ConvertedValue.Should().BeApproximately(328.0839895013, 0.001);
    }
}
