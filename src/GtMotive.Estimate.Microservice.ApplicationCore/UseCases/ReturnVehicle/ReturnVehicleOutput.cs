using GtMotive.Estimate.Microservice.Domain.Rentals;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ReturnVehicle
{
    /// <summary>
    /// Mensaje de salida del caso de uso Devolver vehículo.
    /// </summary>
    public sealed class ReturnVehicleOutput(Rental rental, Vehicle vehicle) : IUseCaseOutput
    {
        /// <summary>
        /// Obtiene el alquiler que se cerró.
        /// </summary>
        public Rental Rental { get; } = rental;

        /// <summary>
        /// Obtiene el vehículo que vuelve a estar disponible.
        /// </summary>
        public Vehicle Vehicle { get; } = vehicle;
    }
}
