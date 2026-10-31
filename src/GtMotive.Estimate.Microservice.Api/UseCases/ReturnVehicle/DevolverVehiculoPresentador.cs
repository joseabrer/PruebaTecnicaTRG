using System;
using GtMotive.Estimate.Microservice.Api.Models;
using GtMotive.Estimate.Microservice.Api.Respuestas;
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

            var alquiler = new AlquilerRespuesta(response.Rental);

            ActionResult = new OkObjectResult(new RespuestaApi<AlquilerRespuesta>(alquiler, Mensajes.VehiculoDevuelto));
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
