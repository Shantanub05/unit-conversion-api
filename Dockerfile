# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files first for better layer caching
COPY UnitConversion.sln .
COPY src/UnitConversion.Api/UnitConversion.Api.csproj src/UnitConversion.Api/
COPY tests/UnitConversion.Api.Tests/UnitConversion.Api.Tests.csproj tests/UnitConversion.Api.Tests/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Build the project
RUN dotnet build -c Release --no-restore

# Run tests
RUN dotnet test -c Release --no-build --verbosity normal

# Publish the application
RUN dotnet publish src/UnitConversion.Api/UnitConversion.Api.csproj -c Release -o /app/publish --no-build

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Create non-root user for security
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copy published output
COPY --from=build /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Expose port
EXPOSE 8080

# Switch to non-root user
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/swagger/v1/swagger.json || exit 1

ENTRYPOINT ["dotnet", "UnitConversion.Api.dll"]
