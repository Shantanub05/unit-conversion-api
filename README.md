# Unit Conversion API

A production-quality **ASP.NET Core Web API** for converting numerical values between different units of measurement. Built with clean architecture principles, the strategy pattern, and comprehensive testing.

## Features

- 🔄 **6 Conversion Categories**: Length, Temperature, Weight/Mass, Area, Volume, Speed
- 📏 **31+ Units** with accurate conversion factors and formulas
- 📖 **Interactive Swagger UI** for API exploration at `/swagger`
- 🔍 **Discovery Endpoints** to list all supported units and categories
- 🏗️ **Strategy Pattern Architecture** designed for extensibility to hundreds of units
- 📝 **Structured Logging** with Serilog for production-grade observability
- ⚠️ **Global Exception Handling** with RFC 9110 ProblemDetails responses
- 🐳 **Docker Support** for one-command local setup
- ✅ **Comprehensive Tests** with xUnit and FluentAssertions
- 🔄 **CI/CD Pipeline** with GitHub Actions

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (or later)
- **OR** [Docker](https://www.docker.com/get-started) (no .NET SDK needed)

## Getting Started

### Option 1: Run with .NET CLI

```bash
# Clone the repository
git clone https://github.com/Shantanub05/unit-conversion-api.git
cd unit-conversion-api

# Restore dependencies
dotnet restore

# Run the API
dotnet run --project src/UnitConversion.Api

# The API will be available at:
# - Swagger UI: https://localhost:5001/swagger
# - API: https://localhost:5001/api
```

### Option 2: Run with Docker

```bash
# Clone the repository
git clone https://github.com/Shantanub05/unit-conversion-api.git
cd unit-conversion-api

# Build and run with Docker Compose
docker-compose up --build

# The API will be available at:
# - Swagger UI: http://localhost:8080/swagger
# - API: http://localhost:8080/api
```

### Run Tests

```bash
dotnet test --verbosity normal
```

## API Endpoints

### Convert Units

```http
GET /api/convert?value={value}&from={fromUnit}&to={toUnit}
```

**Parameters:**

| Parameter | Type   | Required | Description                              |
|-----------|--------|----------|------------------------------------------|
| `value`   | double | Yes      | The numerical value to convert           |
| `from`    | string | Yes      | Source unit name (case-insensitive)       |
| `to`      | string | Yes      | Target unit name (case-insensitive)      |

**Example:**

```bash
# Convert 100 meters to feet
curl "http://localhost:8080/api/convert?value=100&from=meter&to=foot"
```

**Response:**

```json
{
  "originalValue": 100,
  "fromUnit": "meter",
  "convertedValue": 328.0839895013,
  "toUnit": "foot",
  "category": "length"
}
```

**More Examples:**

```bash
# Temperature: Celsius to Fahrenheit
curl "http://localhost:8080/api/convert?value=0&from=celsius&to=fahrenheit"
# → {"convertedValue": 32, ...}

# Weight: Kilograms to Pounds
curl "http://localhost:8080/api/convert?value=1&from=kilogram&to=pound"
# → {"convertedValue": 2.2046226218, ...}

# Speed: km/h to mph
curl "http://localhost:8080/api/convert?value=100&from=kilometers%20per%20hour&to=miles%20per%20hour"
# → {"convertedValue": 62.1371..., ...}
```

### List All Units

```http
GET /api/units
```

Returns all supported units grouped by category.

### List Units by Category

```http
GET /api/units/{category}
```

Returns units for a specific category. Available categories: `length`, `temperature`, `weight`, `area`, `volume`, `speed`.

### Error Responses

All errors are returned as [RFC 9110](https://tools.ietf.org/html/rfc9110) ProblemDetails:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "Unsupported conversion: 'meter' cannot be converted to 'celsius'. They belong to different categories ('length' and 'temperature').",
  "traceId": "0HN8..."
}
```

## Supported Units

| Category    | Units                                                                              |
|-------------|------------------------------------------------------------------------------------|
| **Length**      | meter, kilometer, centimeter, millimeter, mile, yard, foot, inch               |
| **Temperature** | celsius, fahrenheit, kelvin                                                    |
| **Weight**      | kilogram, gram, milligram, pound, ounce, ton, stone                            |
| **Area**        | square meter, square foot, acre, hectare, square kilometer, square mile        |
| **Volume**      | liter, milliliter, gallon, cup, cubic meter, fluid ounce                       |
| **Speed**       | meters per second, kilometers per hour, miles per hour, knot                   |

## Architecture

```
src/UnitConversion.Api/
├── Controllers/           → API endpoints (ConversionController)
├── Models/                → DTOs (ConversionRequest, ConversionResult, UnitInfo)
├── Services/
│   ├── Interfaces/        → IUnitConverter, IConversionService
│   ├── Converters/        → Strategy implementations (one per category)
│   └── ConversionService  → Orchestrator that routes to correct converter
├── Middleware/             → Global exception handling
├── Extensions/            → DI registration
└── Program.cs             → Application entry point
```

### Design Decisions

| Decision | Rationale |
|----------|-----------|
| **Strategy Pattern** | Each conversion category is encapsulated in its own `IUnitConverter` implementation. This enables independent testing, clear separation of concerns, and easy addition of new categories without modifying existing code (Open/Closed Principle). |
| **Base-Unit Normalization** | For linear conversions (all except temperature), values are converted to a base unit first, then to the target. This requires O(n) conversion factors instead of O(n²) pairwise factors, making it scalable to hundreds of units. |
| **Formula-Based Temperature** | Temperature conversions are non-linear (e.g., Celsius to Fahrenheit involves multiplication and addition). The `TemperatureConverter` uses dedicated formulas rather than simple factors. |
| **Case-Insensitive Matching** | All unit names are matched case-insensitively for a better developer experience. |
| **GET Endpoint** | Unit conversion is a read-only, idempotent operation. Using GET with query parameters makes requests cacheable, bookmarkable, and easy to test in a browser. |
| **ProblemDetails Errors** | Following RFC 9110 for structured error responses provides a consistent, machine-readable error format. |
| **Serilog** | Structured logging enables powerful log querying and analysis in production. Minimal overhead with the console sink for this version. |
| **Hardcoded Data** | Per requirements, conversion factors are hardcoded. The architecture supports future migration to a database or configuration file without API changes. |

### Trade-offs

- **Hardcoded vs. Database**: Simplicity and zero infrastructure vs. dynamic unit management. The strategy pattern makes migration straightforward.
- **Singleton Services**: Converters are stateless and thread-safe, so singletons avoid unnecessary allocations. If state were needed, scoped lifetime would be more appropriate.
- **Floating-Point Precision**: Results are rounded to 10 decimal places. For scientific applications, `decimal` type or arbitrary-precision libraries would be preferable.

## Project Structure

```
unit-conversion-api/
├── src/
│   └── UnitConversion.Api/        # Main API project
├── tests/
│   └── UnitConversion.Api.Tests/  # Test project
├── docs/                          # Project documentation
│   ├── requirements.md            # Requirements specification
│   └── implementation-plan.md     # Implementation plan
├── .github/workflows/ci.yml      # CI pipeline
├── Dockerfile                     # Multi-stage Docker build
├── docker-compose.yml             # Docker Compose configuration
├── .editorconfig                  # Code style configuration
├── .gitignore                     # Git ignore rules
├── UnitConversion.sln             # Solution file
└── README.md                     # This file
```

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/add-new-category`)
3. Write tests for your changes
4. Ensure all tests pass (`dotnet test`)
5. Commit your changes (`git commit -m 'Add new conversion category'`)
6. Push to the branch (`git push origin feature/add-new-category`)
7. Open a Pull Request

## License

This project is provided as-is for evaluation purposes.
