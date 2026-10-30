# Build context: the repository root, where docker-compose.yml lives.
#   docker build -f src/GtMotive.Estimate.Microservice.Host/Dockerfile .
# It only needs nuget.org: there is no private feed and no credentials.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Restore first, with only the files that define the dependency graph, so this layer stays cached
# until a package or a project reference changes.
COPY global.json Directory.Build.props Directory.Build.targets NuGet.config ./
COPY src/GtMotive.Estimate.Microservice.Host/GtMotive.Estimate.Microservice.Host.csproj src/GtMotive.Estimate.Microservice.Host/
COPY src/GtMotive.Estimate.Microservice.Api/GtMotive.Estimate.Microservice.Api.csproj src/GtMotive.Estimate.Microservice.Api/
COPY src/GtMotive.Estimate.Microservice.ApplicationCore/GtMotive.Estimate.Microservice.ApplicationCore.csproj src/GtMotive.Estimate.Microservice.ApplicationCore/
COPY src/GtMotive.Estimate.Microservice.Domain/GtMotive.Estimate.Microservice.Domain.csproj src/GtMotive.Estimate.Microservice.Domain/
COPY src/GtMotive.Estimate.Microservice.Infrastructure/GtMotive.Estimate.Microservice.Infrastructure.csproj src/GtMotive.Estimate.Microservice.Infrastructure/
RUN dotnet restore src/GtMotive.Estimate.Microservice.Host/GtMotive.Estimate.Microservice.Host.csproj

COPY .editorconfig stylecop.json ./
COPY src src
RUN dotnet publish src/GtMotive.Estimate.Microservice.Host/GtMotive.Estimate.Microservice.Host.csproj \
    -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "GtMotive.Estimate.Microservice.Host.dll"]
