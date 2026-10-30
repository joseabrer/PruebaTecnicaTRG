using System;
using GtMotive.Estimate.Microservice.Domain.Rentals;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle
{
    /// <summary>
    /// Mensaje de entrada del caso de uso Alquilar vehículo.
    /// </summary>
    public sealed class RentVehicleInput(Guid vehicleId, RenterId renterId) : IUseCaseInput
    {
        /// <summary>
        /// Obtiene el identificador del vehículo que se alquila.
        /// </summary>
        public Guid VehicleId { get; } = vehicleId;

        /// <summary>
        /// Obtiene la persona que alquila el vehículo.
        /// </summary>
        public RenterId RenterId { get; } = renterId;
    }
}
