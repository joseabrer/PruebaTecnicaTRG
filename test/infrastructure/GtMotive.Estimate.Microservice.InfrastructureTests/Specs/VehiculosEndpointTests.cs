using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.CreateVehicle;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using GtMotive.Estimate.Microservice.InfrastructureTests.Infrastructure;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GtMotive.Estimate.Microservice.InfrastructureTests.Specs
{
    /// <summary>
    /// Tests de infraestructura de POST /api/vehiculos: solo se ejercita el host (enrutado, enlace del JSON, validación
    /// del modelo y la respuesta). El caso de uso se sustituye por un doble de prueba, así que no se ejecuta nada por debajo.
    /// </summary>
    public sealed class VehiculosEndpointTests(GenericInfrastructureTestServerFixture fixture) : InfrastructureTestBase(fixture)
    {
        [Fact]
        public async Task PostWithoutBrandIsRejectedBeforeReachingTheUseCase()
        {
            var llamadas = new List<CreateVehicleInput>();
            using var servidor = CrearServidor(llamadas);
            using var cliente = servidor.CreateClient();

            var respuesta = await cliente.PostAsJsonAsync("/api/vehiculos", new { matricula = "ab-123", modelo = "Ibiza", fechaFabricacion = "2025-01-01" });

            respuesta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            using var cuerpo = JsonDocument.Parse(await respuesta.Content.ReadAsStringAsync());
            cuerpo.RootElement.GetProperty("exito").GetBoolean().Should().BeFalse();
            cuerpo.RootElement.GetProperty("mensaje").GetString().Should().Be("Solicitud no válida");
            cuerpo.RootElement.GetProperty("datos").ValueKind.Should().Be(JsonValueKind.Null);
            LeerErrores(cuerpo.RootElement).Should().Contain("El campo Marca es obligatorio.");
            llamadas.Should().BeEmpty();
        }

        [Fact]
        public async Task PostWithAnInvalidDateIsRejectedWithASpanishMessage()
        {
            var llamadas = new List<CreateVehicleInput>();
            using var servidor = CrearServidor(llamadas);
            using var cliente = servidor.CreateClient();

            var respuesta = await cliente.PostAsJsonAsync("/api/vehiculos", new { matricula = "ab-123", marca = "Seat", modelo = "Ibiza", fechaFabricacion = "no-es-fecha" });

            respuesta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            using var cuerpo = JsonDocument.Parse(await respuesta.Content.ReadAsStringAsync());
            LeerErrores(cuerpo.RootElement).Should().ContainSingle().Which.Should().Be("fechaFabricacion: El valor indicado no es válido.");
            llamadas.Should().BeEmpty();
        }

        [Fact]
        public async Task PostWithoutBodyIsRejectedWithASpanishMessage()
        {
            var llamadas = new List<CreateVehicleInput>();
            using var servidor = CrearServidor(llamadas);
            using var cliente = servidor.CreateClient();
            using var contenido = new StringContent(string.Empty, Encoding.UTF8, "application/json");

            var respuesta = await cliente.PostAsync(new Uri("/api/vehiculos", UriKind.Relative), contenido);

            respuesta.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            using var cuerpo = JsonDocument.Parse(await respuesta.Content.ReadAsStringAsync());
            LeerErrores(cuerpo.RootElement).Should().Contain("Hay que enviar un cuerpo en la petición.");
            llamadas.Should().BeEmpty();
        }

        [Fact]
        public async Task PostWithAValidBodyReachesTheUseCaseWithTheMappedInputAndReturnsCreated()
        {
            var llamadas = new List<CreateVehicleInput>();
            using var servidor = CrearServidor(llamadas);
            using var cliente = servidor.CreateClient();

            var respuesta = await cliente.PostAsJsonAsync("/api/vehiculos", new { matricula = " ab-123 ", marca = "Seat", modelo = "Ibiza", fechaFabricacion = "2025-01-01" });

            respuesta.StatusCode.Should().Be(HttpStatusCode.Created);

            var entrada = llamadas.Should().ContainSingle().Subject;
            entrada.LicensePlate.Value.Should().Be("AB-123");
            entrada.Brand.Should().Be("Seat");
            entrada.Model.Should().Be("Ibiza");
            entrada.ManufactureDate.Should().Be(new DateOnly(2025, 1, 1));

            using var cuerpo = JsonDocument.Parse(await respuesta.Content.ReadAsStringAsync());
            cuerpo.RootElement.GetProperty("exito").GetBoolean().Should().BeTrue();
            cuerpo.RootElement.GetProperty("mensaje").GetString().Should().Be("Vehículo creado correctamente.");
            cuerpo.RootElement.GetProperty("errores").ValueKind.Should().Be(JsonValueKind.Null);

            var datos = cuerpo.RootElement.GetProperty("datos");
            datos.GetProperty("matricula").GetString().Should().Be("AB-123");

            // A vehicle is always available when it is created or listed, so a status field would carry no information.
            datos.TryGetProperty("estado", out _).Should().BeFalse();
        }

        [Fact]
        public async Task AConflictIsAnsweredInsideTheEnvelopeWithTheListOfErrors()
        {
            using var servidor = GenericInfrastructureTestServerFixture.CreateServer(servicios =>
                servicios.AddScoped<IUseCase<CreateVehicleInput>>(proveedor =>
                    new CasoDeUsoQueDetectaUnaMatriculaRepetida(proveedor.GetRequiredService<ICreateVehicleOutputPort>())));
            using var cliente = servidor.CreateClient();

            var respuesta = await cliente.PostAsJsonAsync("/api/vehiculos", new { matricula = "ab-123", marca = "Seat", modelo = "Ibiza", fechaFabricacion = "2025-01-01" });

            respuesta.StatusCode.Should().Be(HttpStatusCode.Conflict);
            using var cuerpo = JsonDocument.Parse(await respuesta.Content.ReadAsStringAsync());
            cuerpo.RootElement.GetProperty("exito").GetBoolean().Should().BeFalse();
            cuerpo.RootElement.GetProperty("mensaje").GetString().Should().Be("Conflicto");
            cuerpo.RootElement.GetProperty("datos").ValueKind.Should().Be(JsonValueKind.Null);
            LeerErrores(cuerpo.RootElement).Should().ContainSingle().Which.Should().Be("Ya existe un vehículo con la matrícula AB-123.");
        }

        [Fact]
        public async Task PostWithAnUnsupportedContentTypeIsAnsweredInsideTheEnvelope()
        {
            var llamadas = new List<CreateVehicleInput>();
            using var servidor = CrearServidor(llamadas);
            using var cliente = servidor.CreateClient();
            using var contenido = new StringContent("hola", Encoding.UTF8, "text/plain");

            var respuesta = await cliente.PostAsync(new Uri("/api/vehiculos", UriKind.Relative), contenido);

            respuesta.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
            respuesta.Content.Headers.ContentType.MediaType.Should().Be("application/json");
            using var cuerpo = JsonDocument.Parse(await respuesta.Content.ReadAsStringAsync());
            cuerpo.RootElement.GetProperty("exito").GetBoolean().Should().BeFalse();
            cuerpo.RootElement.GetProperty("mensaje").GetString().Should().Be("Tipo de contenido no admitido");
            cuerpo.RootElement.GetProperty("datos").ValueKind.Should().Be(JsonValueKind.Null);
            LeerErrores(cuerpo.RootElement).Should().ContainSingle();
            llamadas.Should().BeEmpty();
        }

        private static TestServer CrearServidor(List<CreateVehicleInput> llamadas)
        {
            return GenericInfrastructureTestServerFixture.CreateServer(servicios =>
                servicios.AddScoped<IUseCase<CreateVehicleInput>>(proveedor =>
                    new CasoDeUsoQueRegistraLlamadas(proveedor.GetRequiredService<ICreateVehicleOutputPort>(), llamadas)));
        }

        private static List<string> LeerErrores(JsonElement raiz)
        {
            var errores = raiz.GetProperty("errores").EnumerateArray().Select(error => error.GetString()).ToList();

            return errores;
        }

        private sealed class CasoDeUsoQueDetectaUnaMatriculaRepetida(ICreateVehicleOutputPort puertoDeSalida) : IUseCase<CreateVehicleInput>
        {
            public Task Execute(CreateVehicleInput input)
            {
                ArgumentNullException.ThrowIfNull(input);

                puertoDeSalida.ConflictHandle($"Ya existe un vehículo con la matrícula {input.LicensePlate.Value}.");

                return Task.CompletedTask;
            }
        }

        private sealed class CasoDeUsoQueRegistraLlamadas(ICreateVehicleOutputPort puertoDeSalida, List<CreateVehicleInput> llamadas) : IUseCase<CreateVehicleInput>
        {
            public Task Execute(CreateVehicleInput input)
            {
                llamadas.Add(input);

                var vehiculo = Vehicle.Restore(Guid.NewGuid(), input.LicensePlate, input.Brand, input.Model, input.ManufactureDate, VehicleStatus.Available);
                puertoDeSalida.StandardHandle(new CreateVehicleOutput(vehiculo));

                return Task.CompletedTask;
            }
        }
    }
}
