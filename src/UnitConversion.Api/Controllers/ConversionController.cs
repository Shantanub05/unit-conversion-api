using Microsoft.AspNetCore.Mvc;
using UnitConversion.Api.Models;
using UnitConversion.Api.Services.Interfaces;

namespace UnitConversion.Api.Controllers;

/// <summary>
/// Provides endpoints for converting values between units of measurement
/// and discovering supported units.
/// </summary>
[ApiController]
[Route("api")]
[Produces("application/json")]
public class ConversionController : ControllerBase
{
    private readonly IConversionService _conversionService;
    private readonly ILogger<ConversionController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConversionController"/> class.
    /// </summary>
    /// <param name="conversionService">The conversion service for handling unit conversions.</param>
    /// <param name="logger">The logger instance.</param>
    public ConversionController(IConversionService conversionService, ILogger<ConversionController> logger)
    {
        _conversionService = conversionService ?? throw new ArgumentNullException(nameof(conversionService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Converts a numerical value from one unit of measurement to another.
    /// </summary>
    /// <param name="value">The numerical value to convert.</param>
    /// <param name="from">The source unit name (case-insensitive). Example: "meter".</param>
    /// <param name="to">The target unit name (case-insensitive). Example: "foot".</param>
    /// <returns>A <see cref="ConversionResult"/> containing the converted value and metadata.</returns>
    /// <response code="200">The conversion was successful.</response>
    /// <response code="400">The request was invalid (missing parameters or unsupported units).</response>
    [HttpGet("convert")]
    [ProducesResponseType(typeof(ConversionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Convert(
        [FromQuery] double? value,
        [FromQuery] string? from,
        [FromQuery] string? to)
    {
        if (value is null)
        {
            return BadRequest(CreateProblemDetails("The 'value' query parameter is required."));
        }

        if (string.IsNullOrWhiteSpace(from))
        {
            return BadRequest(CreateProblemDetails("The 'from' query parameter is required."));
        }

        if (string.IsNullOrWhiteSpace(to))
        {
            return BadRequest(CreateProblemDetails("The 'to' query parameter is required."));
        }

        _logger.LogInformation(
            "Converting {Value} from {FromUnit} to {ToUnit}",
            value.Value, from, to);

        var result = _conversionService.Convert(value.Value, from.Trim(), to.Trim());

        _logger.LogInformation(
            "Conversion result: {OriginalValue} {FromUnit} = {ConvertedValue} {ToUnit} ({Category})",
            result.OriginalValue, result.FromUnit, result.ConvertedValue, result.ToUnit, result.Category);

        return Ok(result);
    }

    /// <summary>
    /// Lists all supported units grouped by conversion category.
    /// </summary>
    /// <returns>A dictionary mapping category names to their supported units.</returns>
    /// <response code="200">Successfully retrieved all units.</response>
    [HttpGet("units")]
    [ProducesResponseType(typeof(IDictionary<string, IReadOnlyCollection<UnitInfo>>), StatusCodes.Status200OK)]
    public IActionResult GetAllUnits()
    {
        _logger.LogInformation("Retrieving all supported units");

        var units = _conversionService.GetUnits();
        return Ok(units);
    }

    /// <summary>
    /// Lists all supported units for a specific conversion category.
    /// </summary>
    /// <param name="category">The category name (case-insensitive). Example: "length".</param>
    /// <returns>A dictionary with the category and its supported units.</returns>
    /// <response code="200">Successfully retrieved units for the specified category.</response>
    /// <response code="404">The specified category was not found.</response>
    [HttpGet("units/{category}")]
    [ProducesResponseType(typeof(IDictionary<string, IReadOnlyCollection<UnitInfo>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult GetUnitsByCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return BadRequest(CreateProblemDetails("The 'category' parameter is required."));
        }

        _logger.LogInformation("Retrieving units for category: {Category}", category);

        try
        {
            var units = _conversionService.GetUnits(category.Trim());
            return Ok(units);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(CreateProblemDetails(ex.Message, StatusCodes.Status404NotFound));
        }
    }

    private ProblemDetails CreateProblemDetails(string detail, int statusCode = StatusCodes.Status400BadRequest)
    {
        return new ProblemDetails
        {
            Status = statusCode,
            Title = statusCode == StatusCodes.Status400BadRequest ? "Bad Request" : "Not Found",
            Detail = detail,
            Type = statusCode == StatusCodes.Status400BadRequest
                ? "https://tools.ietf.org/html/rfc9110#section-15.5.1"
                : "https://tools.ietf.org/html/rfc9110#section-15.5.5",
            Extensions = { ["traceId"] = HttpContext.TraceIdentifier }
        };
    }
}
