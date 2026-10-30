using System;
using System.Linq;
using GtMotive.Estimate.Microservice.Api.Models;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ListAvailableVehicles;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases.ListAvailableVehicles
{
    public sealed class ListarVehiculosDisponiblesPresentador : IWebApiPresenter, IListAvailableVehiclesOutputPort
    {
        public IActionResult ActionResult { get; private set; }

        public void StandardHandle(ListAvailableVehiclesOutput response)
        {
            ArgumentNullException.ThrowIfNull(response);

            var vehiculos = response.Vehicles.Select(vehiculo => new VehiculoRespuesta(vehiculo)).ToList();

            ActionResult = new OkObjectResult(vehiculos);
        }
    }
}
