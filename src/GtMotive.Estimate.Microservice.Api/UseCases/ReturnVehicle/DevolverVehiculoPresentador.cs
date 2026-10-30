using System;
using GtMotive.Estimate.Microservice.Api.Models;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ReturnVehicle;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases.ReturnVehicle
{
    public sealed class DevolverVehiculoPresentador : IWebApiPresenter, IReturnVehicleOutputPort
    {
        public IActionResult ActionResult { get; private set; }

        public void StandardHandle(ReturnVehicleOutput response)
        {
            ArgumentNullException.ThrowIfNull(response);

            ActionResult = new OkObjectResult(new AlquilerRespuesta(response.Rental));
        }

        public void NotFoundHandle(string message)
        {
            ActionResult = PresenterResults.NotFound(message);
        }

        public void ConflictHandle(string message)
        {
            ActionResult = PresenterResults.Conflict(message);
        }
    }
}
