# Implementation Plan: Unit Conversion API

> **Status**: Ready for review
> **Last Updated**: 2026-06-12

---

## Goal

Build a production-quality ASP.NET Core Web API for unit conversion, demonstrating clean architecture, testability, extensibility, and team-readiness. The API supports 6 conversion categories (length, temperature, weight/mass, area, volume, speed) with a strategy-pattern architecture designed to scale to hundreds of units.

---

## Phase 0: Environment Setup

### 0.1 Install .NET SDK
- Install the latest stable .NET SDK (9.x) on Ubuntu 26.04
- Verify with `dotnet --version`

### 0.2 Initialize Git Repository
- Configure git for the `Shantanub05` GitHub account
- Create the `unit-conversion-api` repo on GitHub
- Set up the remote origin

---

## Phase 1: Project Scaffolding

### 1.1 Solution Structure

```
unit-conversion-api/
├── src/
│   └── UnitConversion.Api/           # Main API project
│       ├── Controllers/              # API controllers
│       ├── Models/                   # DTOs and response models
│       ├── Services/                 # Business logic
│       │   ├── Converters/           # Individual converter implementations
│       │   └── Interfaces/           # IUnitConverter, IConversionService
│       ├── Middleware/               # Global exception handling
│       ├── Extensions/              # Service registration extensions
│       └── Program.cs               # Entry point
├── tests/
│   └── UnitConversion.Api.Tests/     # xUnit test project
│       ├── Controllers/             # Controller integration tests
│       ├── Services/                # Converter unit tests
│       └── Middleware/              # Middleware tests
├── docs/                            # Documentation
│   ├── requirements.md             # Captured requirements
│   └── implementation-plan.md      # This file
├── .github/
│   └── workflows/
│       └── ci.yml                   # GitHub Actions CI pipeline
├── Dockerfile                       # Multi-stage Docker build
├── docker-compose.yml              # Docker Compose configuration
├── .gitignore                      # .NET gitignore
├── .editorconfig                   # Code style consistency
├── UnitConversion.sln              # Solution file
└── README.md                       # Project documentation
```

### 1.2 Create Projects
- Create solution: `dotnet new sln -n UnitConversion`
- Create API project: `dotnet new webapi -n UnitConversion.Api -o src/UnitConversion.Api`
- Create test project: `dotnet new xunit -n UnitConversion.Api.Tests -o tests/UnitConversion.Api.Tests`
- Add projects to solution
- Add project references and NuGet packages

### 1.3 NuGet Packages

**API Project:**
| Package | Purpose |
|---------|---------|
| `Swashbuckle.AspNetCore` | Swagger/OpenAPI documentation |
| `Serilog.AspNetCore` | Structured logging |
| `Serilog.Sinks.Console` | Console log sink |

**Test Project:**
| Package | Purpose |
|---------|---------|
| `FluentAssertions` | Fluent test assertions |
| `Microsoft.AspNetCore.Mvc.Testing` | Integration testing |

---

## Phase 2: Core Architecture (Strategy Pattern)

### 2.1 Interfaces

```csharp
// IUnitConverter — one per category
public interface IUnitConverter
{
    string Category { get; }
    IReadOnlyCollection<string> SupportedUnits { get; }
    bool CanConvert(string fromUnit, string toUnit);
    double Convert(double value, string fromUnit, string toUnit);
}

// IConversionService — orchestrator
public interface IConversionService
{
    ConversionResult Convert(ConversionRequest request);
    IReadOnlyCollection<string> GetCategories();
    IReadOnlyCollection<UnitInfo> GetUnits(string? category = null);
}
```

### 2.2 Models

```csharp
public record ConversionRequest(double Value, string FromUnit, string ToUnit);
public record ConversionResult(double OriginalValue, string FromUnit, double ConvertedValue, string ToUnit, string Category);
public record UnitInfo(string Name, string Category, string Abbreviation);
```

### 2.3 Converter Implementations (6 Categories)

Each converter uses a **base-unit normalization** approach internally:
- Convert `fromUnit → baseUnit → toUnit`
- Temperature uses formula-based conversion (non-linear)

| Category | Base Unit | Units |
|----------|-----------|-------|
| Length | Meter | meter, kilometer, centimeter, millimeter, mile, yard, foot, inch |
| Temperature | Celsius | celsius, fahrenheit, kelvin |
| Weight/Mass | Kilogram | kilogram, gram, milligram, pound, ounce, ton, stone |
| Area | Square Meter | sqmeter, sqfoot, acre, hectare, sqkilometer, sqmile |
| Volume | Liter | liter, milliliter, gallon, cup, cubicmeter, fluidounce |
| Speed | m/s | mps, kmph, mph, knot |

### 2.4 Conversion Service

The `ConversionService` class:
- Receives all `IUnitConverter` implementations via DI
- Routes conversion requests to the correct converter
- Provides unit discovery functionality
- Throws descriptive exceptions for unsupported conversions

---

## Phase 3: API Layer

### 3.1 Endpoints

| Method | Route | Description | Response |
|--------|-------|-------------|----------|
| `GET` | `/api/convert` | Convert a value | `ConversionResult` |
| `GET` | `/api/units` | List all units grouped by category | `Dictionary<string, UnitInfo[]>` |
| `GET` | `/api/units/{category}` | List units for a specific category | `UnitInfo[]` |

**Query Parameters for `/api/convert`:**
| Parameter | Type | Required | Example |
|-----------|------|----------|---------|
| `value` | double | Yes | `100` |
| `from` | string | Yes | `meter` |
| `to` | string | Yes | `foot` |

### 3.2 Response Format

**Success (200):**
```json
{
  "originalValue": 100,
  "fromUnit": "meter",
  "convertedValue": 328.084,
  "toUnit": "foot",
  "category": "length"
}
```

**Error (400):**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "Unsupported conversion: 'meter' to 'celsius'. These units belong to different categories.",
  "traceId": "..."
}
```

### 3.3 Swagger Configuration
- Enable SwaggerUI at `/swagger`
- Add XML documentation comments
- Include example requests/responses

---

## Phase 4: Cross-Cutting Concerns

### 4.1 Global Exception Handling Middleware
- Catch unhandled exceptions
- Return structured ProblemDetails responses
- Log exceptions with Serilog

### 4.2 Serilog Configuration
- Structured JSON logging
- Console sink for development
- Request logging middleware
- Correlation IDs

### 4.3 Input Validation
- Validate required parameters
- Case-insensitive unit matching
- Meaningful error messages

---

## Phase 5: Testing

### 5.1 Unit Tests (Converters)
- Test each converter with known conversion pairs
- Test edge cases (0 values, negative values, same unit)
- Test unsupported unit handling
- Test case insensitivity

### 5.2 Unit Tests (Service Layer)
- Test routing to correct converter
- Test category listing
- Test unit discovery

### 5.3 Integration Tests
- Test full HTTP request/response cycle via `WebApplicationFactory`
- Test all endpoints with valid inputs
- Test error responses for invalid inputs
- Test Swagger endpoint availability

### 5.4 Test Coverage Target
- Aim for **90%+** code coverage on business logic

---

## Phase 6: DevOps & Documentation

### 6.1 Dockerfile
- Multi-stage build (build → publish → runtime)
- Use official .NET SDK and runtime images
- Optimize for small image size

### 6.2 docker-compose.yml
- Single service configuration
- Port mapping (8080:8080)
- Health check

### 6.3 GitHub Actions CI
```yaml
Triggers: push to main, pull requests
Steps: checkout → setup .NET → restore → build → test → (optional) Docker build
```

### 6.4 README.md
- Project description
- Prerequisites
- Run instructions (dotnet CLI + Docker)
- API endpoint documentation with examples
- Architecture overview with diagram
- Design decisions and trade-offs

### 6.5 Additional Files
- `.gitignore` — standard .NET template
- `.editorconfig` — consistent code formatting
- `CONTRIBUTING.md` — contribution guidelines (team-readiness)

---

## Phase 7: Final Polish & Deployment

### 7.1 Code Quality
- Ensure consistent naming conventions
- Remove any company name references
- Add XML doc comments on all public APIs
- Verify all tests pass

### 7.2 Git Hygiene
- Clean commit history with meaningful messages
- Push to `Shantanub05/unit-conversion-api`
- Verify GitHub Actions CI passes

### 7.3 Verification Checklist
- [ ] `dotnet build` succeeds with no warnings
- [ ] `dotnet test` — all tests pass
- [ ] `docker-compose up` — API starts and responds
- [ ] Swagger UI loads at `/swagger`
- [ ] All 6 conversion categories work correctly
- [ ] Discovery endpoints return correct data
- [ ] Error handling returns proper ProblemDetails
- [ ] README.md is complete and accurate
- [ ] No company names in code or docs
- [ ] CI pipeline runs successfully on GitHub

---

## Design Decisions & Trade-offs

| Decision | Rationale |
|----------|-----------|
| **Strategy pattern over flat registry** | Better encapsulation, each category can have custom logic (temperature formulas vs linear factors), easier to test in isolation |
| **Base-unit normalization** | O(n) conversion factors needed instead of O(n²) for pairwise. Temperature is the exception with custom formulas |
| **GET with query params** | RESTful, cacheable, easy to test in browser/curl. Conversion is a read operation, not a mutation |
| **Hardcoded data** | Per requirements. Architecture supports future DB/config migration without API changes |
| **Serilog over built-in** | Structured logging is essential for production debugging; minimal overhead |
| **Docker + docker-compose** | One-command local setup (`docker-compose up`), no .NET SDK needed for reviewers |
