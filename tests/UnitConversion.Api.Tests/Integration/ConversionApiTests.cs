using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using UnitConversion.Api.Models;

namespace UnitConversion.Api.Tests.Integration;

public class ConversionApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public ConversionApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    [Fact]
    public async Task GetConvert_ValidRequest_Returns200()
    {
        var response = await _client.GetAsync("/api/convert?value=100&from=meter&to=foot");
        
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ConversionResult>(content, _jsonOptions);
        
        result.Should().NotBeNull();
        result!.ConvertedValue.Should().BeApproximately(328.0839895013, 0.001);
    }

    [Fact]
    public async Task GetConvert_MissingValue_Returns400()
    {
        var response = await _client.GetAsync("/api/convert?from=meter&to=foot");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetConvert_MissingFromUnit_Returns400()
    {
        var response = await _client.GetAsync("/api/convert?value=100&to=foot");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetConvert_MissingToUnit_Returns400()
    {
        var response = await _client.GetAsync("/api/convert?value=100&from=meter");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetConvert_CrossCategoryUnits_Returns400()
    {
        var response = await _client.GetAsync("/api/convert?value=100&from=meter&to=celsius");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetConvert_CaseInsensitive_Returns200()
    {
        var response = await _client.GetAsync("/api/convert?value=100&from=METER&to=Foot");
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task GetConvert_TemperatureConversion_Returns200()
    {
        var response = await _client.GetAsync("/api/convert?value=0&from=celsius&to=fahrenheit");
        
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ConversionResult>(content, _jsonOptions);
        
        result!.ConvertedValue.Should().Be(32);
    }

    [Fact]
    public async Task GetUnits_ReturnsAllCategories()
    {
        var response = await _client.GetAsync("/api/units");
        
        response.IsSuccessStatusCode.Should().BeTrue();
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Dictionary<string, List<UnitInfo>>>(content, _jsonOptions);
        
        result.Should().NotBeNull();
        result!.Keys.Should().HaveCount(6);
    }

    [Fact]
    public async Task GetUnitsByCategory_ValidCategory_Returns200()
    {
        var response = await _client.GetAsync("/api/units/length");
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task GetUnitsByCategory_InvalidCategory_Returns404()
    {
        var response = await _client.GetAsync("/api/units/nonexistent");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SwaggerEndpoint_Returns200()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");
        response.IsSuccessStatusCode.Should().BeTrue();
    }
}
