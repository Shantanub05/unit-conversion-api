using UnitConversion.Api.Services;
using UnitConversion.Api.Services.Converters;
using UnitConversion.Api.Services.Interfaces;

namespace UnitConversion.Api.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to register unit conversion services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all unit converters and the conversion orchestration service
    /// into the dependency injection container as singletons.
    /// </summary>
    /// <param name="services">The service collection to add registrations to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for method chaining.</returns>
    /// <remarks>
    /// Registers the following converters:
    /// <list type="bullet">
    ///   <item><description><see cref="LengthConverter"/> — meters, feet, inches, etc.</description></item>
    ///   <item><description><see cref="TemperatureConverter"/> — Celsius, Fahrenheit, Kelvin</description></item>
    ///   <item><description><see cref="WeightConverter"/> — kilograms, pounds, ounces, etc.</description></item>
    ///   <item><description><see cref="AreaConverter"/> — square meters, acres, hectares, etc.</description></item>
    ///   <item><description><see cref="VolumeConverter"/> — liters, gallons, cups, etc.</description></item>
    ///   <item><description><see cref="SpeedConverter"/> — m/s, km/h, mph, etc.</description></item>
    /// </list>
    /// All registrations are singletons since converters hold no mutable state.
    /// </remarks>
    public static IServiceCollection AddConversionServices(this IServiceCollection services)
    {
        // Register all unit converters
        services.AddSingleton<IUnitConverter, LengthConverter>();
        services.AddSingleton<IUnitConverter, TemperatureConverter>();
        services.AddSingleton<IUnitConverter, WeightConverter>();
        services.AddSingleton<IUnitConverter, AreaConverter>();
        services.AddSingleton<IUnitConverter, VolumeConverter>();
        services.AddSingleton<IUnitConverter, SpeedConverter>();

        // Register the conversion orchestration service
        services.AddSingleton<IConversionService, ConversionService>();

        return services;
    }
}
