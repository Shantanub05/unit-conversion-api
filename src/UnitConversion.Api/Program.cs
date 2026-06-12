using Serilog;
using UnitConversion.Api.Extensions;
using UnitConversion.Api.Middleware;

// Configure Serilog early for startup logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Unit Conversion API");

    var builder = WebApplication.CreateBuilder(args);

    // Configure Serilog from configuration
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console(outputTemplate:
            "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"));

    // Add services to the container
    builder.Services.AddControllers();
    builder.Services.AddConversionServices();

    // Configure Swagger/OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
        {
            Title = "Unit Conversion API",
            Version = "v1",
            Description = "A RESTful API for converting numerical values between different units of measurement. " +
                          "Supports length, temperature, weight/mass, area, volume, and speed conversions.",
            Contact = new Microsoft.OpenApi.OpenApiContact
            {
                Name = "API Support",
            }
        });

        // Include XML comments for Swagger documentation
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline

    // Global exception handling (must be first in pipeline)
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    // Serilog request logging
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    });

    // Enable Swagger in all environments for this demo
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Unit Conversion API v1");
        options.DocumentTitle = "Unit Conversion API - Swagger UI";
    });

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    // Redirect root to Swagger UI
    app.MapGet("/", () => Results.Redirect("/swagger"));

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Required for integration testing with WebApplicationFactory
/// <summary>
/// Partial Program class to enable integration testing with WebApplicationFactory.
/// </summary>
public partial class Program { }
