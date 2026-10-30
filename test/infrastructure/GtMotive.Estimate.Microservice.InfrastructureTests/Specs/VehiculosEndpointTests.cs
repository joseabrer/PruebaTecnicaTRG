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
            cuerpo.RootElement.GetProperty("title").GetString().Should().Be("Solicitud no válida");
            LeerCampos(cuerpo.RootElement).Should().Contain("Marca");
            LeerMensajes(cuerpo.RootElement).Should().Contain("El campo Marca es obligatorio.");
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
            LeerMensajes(cuerpo.RootElement).Should().ContainSingle().Which.Should().Be("El valor indicado no es válido.");
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
            LeerMensajes(cuerpo.RootElement).Should().Contain("Hay que enviar un cuerpo en la petición.");
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
            cuerpo.RootElement.GetProperty("matricula").GetString().Should().Be("AB-123");

            // A vehicle is always available when it is created or listed, so a status field would carry no information.
            cuerpo.RootElement.TryGetProperty("estado", out _).Should().BeFalse();
        }

        private static TestServer CrearServidor(List<CreateVehicleInput> llamadas)
        {
            return GenericInfrastructureTestServerFixture.CreateServer(servicios =>
                servicios.AddScoped<IUseCase<CreateVehicleInput>>(proveedor =>
                    new CasoDeUsoQueRegistraLlamadas(proveedor.GetRequiredService<ICreateVehicleOutputPort>(), llamadas)));
        }

        private static List<string> LeerCampos(JsonElement raiz)
        {
            var campos = raiz.GetProperty("errors").EnumerateObject().Select(campo => campo.Name).ToList();

            return campos;
        }

        private static List<string> LeerMensajes(JsonElement raiz)
        {
            var mensajes = raiz.GetProperty("errors").EnumerateObject()
                .SelectMany(campo => campo.Value.EnumerateArray().Select(mensaje => mensaje.GetString()))
                .ToList();

            return mensajes;
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
