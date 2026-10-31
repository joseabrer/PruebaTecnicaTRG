using System.Text.Json;
using FluentAssertions;
using GtMotive.Estimate.Microservice.Api.Models;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.Api
{
    /// <summary>
    /// Tests unitarios del sobre RespuestaApi: la misma forma para una respuesta de éxito y para una de error.
    /// </summary>
    public sealed class RespuestaApiTests
    {
        private static readonly JsonSerializerOptions Opciones = new(JsonSerializerDefaults.Web);

        [Fact]
        public void ASuccessResponseCarriesTheDataAndNoErrors()
        {
            var respuesta = new RespuestaApi<string>("hola", "Hecho.");

            respuesta.Exito.Should().BeTrue();
            respuesta.Mensaje.Should().Be("Hecho.");
            respuesta.Datos.Should().Be("hola");
            respuesta.Errores.Should().BeNull();
        }

        [Fact]
        public void AnErrorResponseCarriesTheErrorsAndNoData()
        {
            var respuesta = new RespuestaApi<object>(
                "No encontrado",
                ["El vehículo no existe."]);

            respuesta.Exito.Should().BeFalse();
            respuesta.Mensaje.Should().Be("No encontrado");
            respuesta.Datos.Should().BeNull();
            respuesta.Errores.Should().ContainSingle().Which.Should().Be("El vehículo no existe.");
        }

        [Fact]
        public void BothKindsOfResponseSerializeTheSameFourFields()
        {
            var exito = new RespuestaApi<int>(5, "Hecho.");
            var error = new RespuestaApi<object>(
                "Conflicto",
                ["Ya existe."]);

            JsonSerializer.Serialize(exito, Opciones)
                .Should().Be("""{"exito":true,"mensaje":"Hecho.","datos":5,"errores":null}""");
            JsonSerializer.Serialize(error, Opciones)
                .Should().Be("""{"exito":false,"mensaje":"Conflicto","datos":null,"errores":["Ya existe."]}""");
        }
    }
}
