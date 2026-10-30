using System.Collections.Generic;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Api.Models;
using GtMotive.Estimate.Microservice.Api.Respuestas;
using GtMotive.Estimate.Microservice.Api.UseCases.CreateVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.ListAvailableVehicles;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.Controllers
{
    /// <summary>
    /// Flota de vehículos.
    /// </summary>
    [ApiController]
    [Route("api/vehiculos")]
    public sealed class VehiculosController(IMediator mediador, IAppLogger<VehiculosController> registro) : ControllerBase
    {
        private readonly IMediator _mediador = mediador;
        private readonly IAppLogger<VehiculosController> _registro = registro;

        /// <summary>
        /// Añade un vehículo a la flota.
        /// </summary>
        /// <param name="solicitud">Vehículo que se añade.</param>
        /// <returns>El nuevo vehículo.</returns>
        [HttpPost]
        [RespuestasApi(typeof(VehiculoRespuesta), StatusCodes.Status201Created, Conflicto = true)]
        public async Task<IActionResult> Crear([FromBody] CrearVehiculoSolicitud solicitud)
        {
            _registro.LogDebug("Entrando en Crear. Solicitud: {@solicitud}", solicitud);

            var presentador = await _mediador.Send(solicitud);
            return presentador.ActionResult;
        }

        /// <summary>
        /// Lista los vehículos que se pueden alquilar ahora mismo.
        /// </summary>
        /// <returns>Los vehículos disponibles.</returns>
        [HttpGet("disponibles")]
        [RespuestasApi(typeof(List<VehiculoRespuesta>), SolicitudInvalida = false)]
        public async Task<IActionResult> ListarDisponibles()
        {
            _registro.LogDebug("Entrando en ListarDisponibles.");

            var presentador = await _mediador.Send(new ListarVehiculosDisponiblesSolicitud());
            return presentador.ActionResult;
        }
    }
}
