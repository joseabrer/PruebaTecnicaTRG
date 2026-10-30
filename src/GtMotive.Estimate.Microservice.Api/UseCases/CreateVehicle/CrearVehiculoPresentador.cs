using System;
using GtMotive.Estimate.Microservice.Api.Models;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.CreateVehicle;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases.CreateVehicle
{
    public sealed class CrearVehiculoPresentador : IWebApiPresenter, ICreateVehicleOutputPort
    {
        public IActionResult ActionResult { get; private set; }

        public void StandardHandle(CreateVehicleOutput response)
        {
            ArgumentNullException.ThrowIfNull(response);

            ActionResult = new ObjectResult(new VehiculoRespuesta(response.Vehicle))
            {
                StatusCode = StatusCodes.Status201Created
            };
        }

        public void ConflictHandle(string message)
        {
            ActionResult = PresenterResults.Conflict(message);
        }
    }
}
