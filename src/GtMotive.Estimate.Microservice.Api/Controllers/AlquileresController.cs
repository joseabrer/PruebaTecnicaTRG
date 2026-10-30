using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Api.Models;
using GtMotive.Estimate.Microservice.Api.Respuestas;
using GtMotive.Estimate.Microservice.Api.UseCases.RentVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.ReturnVehicle;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.Controllers
{
    /// <summary>
    /// Alquileres de vehículos.
    /// </summary>
    [ApiController]
    [Route("api/alquileres")]
    public sealed class AlquileresController(IMediator mediador, IAppLogger<AlquileresController> registro) : ControllerBase
    {
        private readonly IMediator _mediador = mediador;
        private readonly IAppLogger<AlquileresController> _registro = registro;

        /// <summary>
        /// Alquila un vehículo a una persona que no tiene otro vehículo alquilado.
        /// </summary>
        /// <param name="solicitud">Vehículo y persona.</param>
        /// <returns>El nuevo alquiler.</returns>
        [HttpPost]
        [RespuestasApi(typeof(AlquilerRespuesta), StatusCodes.Status201Created, NoEncontrado = true, Conflicto = true)]
        public async Task<IActionResult> Alquilar([FromBody] AlquilarVehiculoSolicitud solicitud)
        {
            _registro.LogDebug("Entrando en Alquilar. Solicitud: {@solicitud}", solicitud);

            var presentador = await _mediador.Send(solicitud);
            return presentador.ActionResult;
        }

        /// <summary>
        /// Devuelve un vehículo alquilado y cierra su alquiler.
        /// </summary>
        /// <param name="alquilerId">Identificador del alquiler que se cierra.</param>
        /// <returns>El alquiler cerrado.</returns>
        [HttpPost("{alquilerId:guid}/devolucion")]
        [RespuestasApi(typeof(AlquilerRespuesta), SolicitudInvalida = false, NoEncontrado = true, Conflicto = true)]
        public async Task<IActionResult> Devolver(Guid alquilerId)
        {
            _registro.LogDebug("Entrando en Devolver. AlquilerId: {alquilerId}", alquilerId);

            var presentador = await _mediador.Send(new DevolverVehiculoSolicitud(alquilerId));
            return presentador.ActionResult;
        }
    }
}
