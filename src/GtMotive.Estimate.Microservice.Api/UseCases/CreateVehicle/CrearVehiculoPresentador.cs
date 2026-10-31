using System;
using GtMotive.Estimate.Microservice.Api.Models;
using GtMotive.Estimate.Microservice.Api.Respuestas;
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

            var vehiculo = new VehiculoRespuesta(response.Vehicle);

            ActionResult = new ObjectResult(new RespuestaApi<VehiculoRespuesta>(vehiculo, Mensajes.VehiculoCreado))
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
