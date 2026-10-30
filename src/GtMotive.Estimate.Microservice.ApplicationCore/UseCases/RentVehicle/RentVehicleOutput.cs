using GtMotive.Estimate.Microservice.Domain.Rentals;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle
{
    /// <summary>
    /// Mensaje de salida del caso de uso Alquilar vehículo.
    /// </summary>
    public sealed class RentVehicleOutput(Rental rental, Vehicle vehicle) : IUseCaseOutput
    {
        /// <summary>
        /// Obtiene el alquiler que se inició.
        /// </summary>
        public Rental Rental { get; } = rental;

        /// <summary>
        /// Obtiene el vehículo que se alquiló.
        /// </summary>
        public Vehicle Vehicle { get; } = vehicle;
    }
}
