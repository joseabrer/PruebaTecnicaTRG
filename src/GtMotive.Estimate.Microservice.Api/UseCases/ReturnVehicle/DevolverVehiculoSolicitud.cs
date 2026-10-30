using System;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.ReturnVehicle
{
    /// <summary>
    /// Devuelve un vehículo alquilado y cierra su alquiler.
    /// </summary>
    public sealed class DevolverVehiculoSolicitud(Guid alquilerId) : IRequest<IWebApiPresenter>
    {
        /// <summary>
        /// Obtiene el identificador del alquiler que se cierra.
        /// </summary>
        public Guid AlquilerId { get; } = alquilerId;
    }
}
