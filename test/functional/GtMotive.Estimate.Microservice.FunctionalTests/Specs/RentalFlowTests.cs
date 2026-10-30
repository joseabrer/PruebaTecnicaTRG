using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.Api.Models;
using GtMotive.Estimate.Microservice.Api.UseCases;
using GtMotive.Estimate.Microservice.Api.UseCases.CreateVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.ListAvailableVehicles;
using GtMotive.Estimate.Microservice.Api.UseCases.RentVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.ReturnVehicle;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace GtMotive.Estimate.Microservice.FunctionalTests.Specs
{
    /// <summary>
    /// Tests funcionales: toda la aplicación excepto el host. Cada llamada pasa por el manejador de la petición, el
    /// caso de uso, el dominio, los repositorios de MongoDB y el presenter reales, contra un MongoDB real.
    /// </summary>
    public sealed class RentalFlowTests(CompositionRootTestFixture fixture) : FunctionalTestBase(fixture)
    {
        [Fact]
        public async Task ARentedVehicleIsHiddenUntilReturnedAndAPersonCannotRentTwoAtOnce()
        {
            var sufijo = NuevoSufijo();
            var cliente = $"cliente-{sufijo}";

            var primero = Modelo<VehiculoRespuesta>(await Enviar(NuevoVehiculo($"A-{sufijo}")), 201);
            var segundo = Modelo<VehiculoRespuesta>(await Enviar(NuevoVehiculo($"B-{sufijo}")), 201);

            var alquiler = Modelo<AlquilerRespuesta>(await Enviar(new AlquilarVehiculoSolicitud { VehiculoId = primero.Id, ClienteId = cliente }), 201);
            (await IdsDisponibles()).Should().NotContain(primero.Id).And.Contain(segundo.Id);

            (await Enviar(new AlquilarVehiculoSolicitud { VehiculoId = segundo.Id, ClienteId = cliente })).Should().BeOfType<ConflictObjectResult>();

            var devuelto = Modelo<AlquilerRespuesta>(await Enviar(new DevolverVehiculoSolicitud(alquiler.Id)), 200);
            devuelto.Activo.Should().BeFalse();
            (await IdsDisponibles()).Should().Contain(primero.Id);

            Modelo<AlquilerRespuesta>(await Enviar(new AlquilarVehiculoSolicitud { VehiculoId = primero.Id, ClienteId = cliente }), 201);
        }

        [Fact]
        public async Task OnlyOneOfSeveralSimultaneousRentalsOfTheSameVehicleSucceeds()
        {
            var sufijo = NuevoSufijo();
            var vehiculo = Modelo<VehiculoRespuesta>(await Enviar(NuevoVehiculo($"C-{sufijo}")), 201);

            var resultados = await Task.WhenAll(Enumerable.Range(0, 8).Select(i =>
                Enviar(new AlquilarVehiculoSolicitud { VehiculoId = vehiculo.Id, ClienteId = $"persona-{sufijo}-{i}" })));

            resultados.Count(r => r is ObjectResult { StatusCode: 201 }).Should().Be(1);
            resultados.Count(r => r is ConflictObjectResult).Should().Be(7);
        }

        [Fact]
        public async Task AVehicleOlderThanFiveYearsIsRejectedAndNeverStored()
        {
            var sufijo = NuevoSufijo();
            var demasiadoViejo = NuevoVehiculo($"D-{sufijo}");
            demasiadoViejo.FechaFabricacion = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-6);

            var accion = () => Enviar(demasiadoViejo);

            await accion.Should().ThrowAsync<VehicleTooOldException>();

            // Nothing was stored: the same license plate can still be registered with an acceptable date.
            Modelo<VehiculoRespuesta>(await Enviar(NuevoVehiculo($"D-{sufijo}")), 201);
        }

        [Fact]
        public async Task AVehicleThatAgedPastFiveYearsIsNeitherListedNorRented()
        {
            var sufijo = NuevoSufijo();
            var hoy = DateOnly.FromDateTime(DateTime.UtcNow);

            // Both were valid when they joined the fleet; one of them has since passed the five-year limit by a day.
            var envejecido = Vehicle.Restore(Guid.NewGuid(), LicensePlate.Create($"E-{sufijo}"), "Seat", "Ibiza", hoy.AddYears(-5).AddDays(-1), VehicleStatus.Available);
            var enElLimite = Vehicle.Restore(Guid.NewGuid(), LicensePlate.Create($"F-{sufijo}"), "Seat", "Ibiza", hoy.AddYears(-5), VehicleStatus.Available);
            await Fixture.UsingRepository<IVehicleRepository>(async repositorio =>
            {
                await repositorio.Add(envejecido);
                await repositorio.Add(enElLimite);
            });

            (await IdsDisponibles()).Should().Contain(enElLimite.Id).And.NotContain(envejecido.Id);

            var cliente = $"cliente-{sufijo}";
            (await Enviar(new AlquilarVehiculoSolicitud { VehiculoId = envejecido.Id, ClienteId = cliente })).Should().BeOfType<ConflictObjectResult>();

            // The refused rental left nothing behind: the person can still rent a vehicle that is within the limit.
            Modelo<AlquilerRespuesta>(await Enviar(new AlquilarVehiculoSolicitud { VehiculoId = enElLimite.Id, ClienteId = cliente }), 201);
        }

        private static string NuevoSufijo()
        {
            return Guid.NewGuid().ToString("N")[..8];
        }

        private static CrearVehiculoSolicitud NuevoVehiculo(string matricula)
        {
            return new CrearVehiculoSolicitud
            {
                Matricula = matricula,
                Marca = "Seat",
                Modelo = "Ibiza",
                FechaFabricacion = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-1)
            };
        }

        private static TModelo Modelo<TModelo>(IActionResult resultado, int codigoEsperado)
        {
            var resultadoObjeto = resultado.Should().BeAssignableTo<ObjectResult>().Subject;
            resultadoObjeto.StatusCode.Should().Be(codigoEsperado);

            return resultadoObjeto.Value.Should().BeOfType<TModelo>().Subject;
        }

        private async Task<IActionResult> Enviar<TSolicitud>(TSolicitud solicitud)
            where TSolicitud : IRequest<IWebApiPresenter>
        {
            IActionResult resultado = null;

            await Fixture.UsingHandlerForRequestResponse<TSolicitud, IWebApiPresenter>(async manejador =>
            {
                var presentador = await manejador.Handle(solicitud, CancellationToken.None);
                resultado = presentador.ActionResult;
            });

            return resultado;
        }

        private async Task<List<Guid>> IdsDisponibles()
        {
            var vehiculos = Modelo<List<VehiculoRespuesta>>(await Enviar(new ListarVehiculosDisponiblesSolicitud()), 200);

            var ids = vehiculos.Select(v => v.Id).ToList();

            return ids;
        }
    }
}
