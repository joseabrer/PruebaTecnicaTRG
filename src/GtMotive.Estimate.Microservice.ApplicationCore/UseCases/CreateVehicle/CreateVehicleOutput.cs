using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.CreateVehicle
{
    /// <summary>
    /// Mensaje de salida del caso de uso Crear vehículo.
    /// </summary>
    public sealed class CreateVehicleOutput(Vehicle vehicle) : IUseCaseOutput
    {
        /// <summary>
        /// Obtiene el vehículo que se añadió a la flota.
        /// </summary>
        public Vehicle Vehicle { get; } = vehicle;
    }
}
