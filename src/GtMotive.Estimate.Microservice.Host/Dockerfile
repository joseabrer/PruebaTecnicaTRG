# Contexto de construcción: la raíz del repositorio, donde está docker-compose.yml.
#   docker build -f src/GtMotive.Estimate.Microservice.Host/Dockerfile .
# Solo necesita nuget.org: no hay ningún feed privado ni credenciales.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Primero se restaura, con solo los ficheros que definen el grafo de dependencias, para que esta capa siga en caché
# hasta que cambie un paquete o una referencia entre proyectos.
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
