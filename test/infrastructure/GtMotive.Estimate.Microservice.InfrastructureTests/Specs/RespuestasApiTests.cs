using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GtMotive.Estimate.Microservice.Api.Models;
using GtMotive.Estimate.Microservice.InfrastructureTests.Infrastructure;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GtMotive.Estimate.Microservice.InfrastructureTests.Specs
{
    /// <summary>
    /// Tests del atributo reutilizable RespuestasApi: cada acción debe publicar, mediante los mismos metadatos que lee Swagger,
    /// exactamente las respuestas que declara su atributo, todas dentro del sobre RespuestaApi.
    /// </summary>
    public sealed class RespuestasApiTests(GenericInfrastructureTestServerFixture fixture) : InfrastructureTestBase(fixture)
    {
        [Theory]
        [InlineData("POST", "api/vehiculos", new[] { 201, 400, 409 })]
        [InlineData("GET", "api/vehiculos/disponibles", new[] { 200 })]
        [InlineData("POST", "api/alquileres", new[] { 201, 400, 404, 409 })]
        [InlineData("POST", "api/alquileres/{alquilerId}/devolucion", new[] { 200, 404, 409 })]
        public void EachActionPublishesTheResponsesDeclaredByItsAttribute(string metodo, string ruta, int[] codigosEsperados)
        {
            var accion = BuscarAccion(metodo, ruta);

            accion.SupportedResponseTypes.Select(respuesta => respuesta.StatusCode).Should().BeEquivalentTo(codigosEsperados);
            var todosLosErroresVanEnElSobre = accion.SupportedResponseTypes
                .Where(respuesta => respuesta.StatusCode >= 400)
                .All(respuesta => respuesta.Type == typeof(RespuestaApi<object>));

            todosLosErroresVanEnElSobre.Should().BeTrue();
        }

        [Fact]
        public void TheSuccessResponseIsTheEnvelopeAroundTheTypeGivenToTheAttribute()
        {
            var alquilar = BuscarAccion("POST", "api/alquileres");
            var crear = BuscarAccion("POST", "api/vehiculos");
            var listar = BuscarAccion("GET", "api/vehiculos/disponibles");

            alquilar.SupportedResponseTypes.Single(respuesta => respuesta.StatusCode == 201).Type.Should().Be<RespuestaApi<AlquilerRespuesta>>();
            crear.SupportedResponseTypes.Single(respuesta => respuesta.StatusCode == 201).Type.Should().Be<RespuestaApi<VehiculoRespuesta>>();
            listar.SupportedResponseTypes.Single(respuesta => respuesta.StatusCode == 200).Type.Should().Be<RespuestaApi<List<VehiculoRespuesta>>>();
        }

        private ApiDescription BuscarAccion(string metodo, string ruta)
        {
            var proveedor = Fixture.Server.Services.GetRequiredService<IApiDescriptionGroupCollectionProvider>();

            return proveedor.ApiDescriptionGroups.Items
                .SelectMany(grupo => grupo.Items)
                .Single(accion => accion.HttpMethod == metodo && accion.RelativePath == ruta);
        }
    }
}
