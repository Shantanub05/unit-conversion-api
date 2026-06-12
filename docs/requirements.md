# Technical Challenge: Unit Conversion API — Requirements

> **Source**: Software_Engineer_Test.docx
> **Date Captured**: 2026-06-12

---

## Overview

Build an **ASP.NET Core Web API** that allows callers to convert numerical values between different units of measurement (e.g., meters to feet, Celsius to Fahrenheit, kilograms to pounds).

---

## Functional Requirements

| ID   | Requirement | Priority |
|------|-------------|----------|
| FR-1 | The HTTP RESTful API must expose an endpoint that allows callers to convert a number from one unit to another. | **Must** |
| FR-2 | Support **length** conversion category (meters, feet, inches, kilometers, miles, yards, centimeters, millimeters). | **Must** |
| FR-3 | Support **temperature** conversion category (Celsius, Fahrenheit, Kelvin). | **Must** |
| FR-4 | Support **weight/mass** conversion category (kilograms, pounds, ounces, grams, milligrams, tons, stones). | **Must** |
| FR-5 | Support **area** conversion category (square meters, square feet, acres, hectares, square kilometers, square miles). | **Should** |
| FR-6 | Support **volume** conversion category (liters, gallons, cups, milliliters, cubic meters, fluid ounces). | **Should** |
| FR-7 | Support **speed** conversion category (m/s, km/h, mph, knots). | **Should** |
| FR-8 | Units, conversion factors, and other required data can be hardcoded in this version. | **Must** |
| FR-9 | The system should be designed to support hundreds of units and conversion types in the future. | **Must** |
| FR-10 | Provide a discovery endpoint to list all supported units and categories (`GET /api/units`, `GET /api/units/{category}`). | **Should** |

---

## Technical Requirements

| ID   | Requirement | Priority |
|------|-------------|----------|
| TR-1 | Use **ASP.NET Core** (latest stable version). | **Must** |
| TR-2 | The solution must be **runnable locally** with clear instructions in README.md. | **Must** |
| TR-3 | Setup as a **real-world project** maintained by a team of multiple developers. | **Must** |
| TR-4 | Include necessary configuration files, documentation, and clear project structure. | **Must** |

---

## Deliverables

| ID   | Deliverable |
|------|-------------|
| D-1  | A **GitHub repository** containing the complete solution. |
| D-2  | A **README.md** at the root with: brief description, run instructions, design decisions/trade-offs. |

---

## Design Decisions (Resolved via Interview)

| Decision | Choice |
|----------|--------|
| **Conversion categories** | 6 categories: length, temperature, weight/mass, area, volume, speed |
| **Architecture pattern** | Strategy pattern — each category gets its own converter class implementing `IUnitConverter` |
| **API endpoint design** | `GET /api/convert?value=100&from=meter&to=foot` (query parameters) |
| **Discovery endpoint** | Yes — `GET /api/units` and `GET /api/units/{category}` |
| **API documentation** | Swagger/OpenAPI with SwaggerUI at `/swagger` |
| **Testing framework** | xUnit with FluentAssertions |
| **Error handling & logging** | Serilog structured logging + global exception handling middleware |
| **Docker support** | Yes — Dockerfile + docker-compose.yml |
| **CI/CD** | GitHub Actions workflow (build, test, validate) |
| **Repository name** | `unit-conversion-api` |
| **GitHub account** | Shantanub05 |

---

## Constraints

- **DO NOT** mention any company names in code, documentation, or comments.
- AI tools are explicitly allowed.
