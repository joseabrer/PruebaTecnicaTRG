using System.Collections.Generic;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ListAvailableVehicles
{
    /// <summary>
    /// Mensaje de salida del caso de uso Listar vehículos disponibles.
    /// </summary>
    public sealed class ListAvailableVehiclesOutput(IReadOnlyCollection<Vehicle> vehicles) : IUseCaseOutput
    {
        /// <summary>
        /// Obtiene los vehículos que se pueden alquilar ahora mismo.
        /// </summary>
        public IReadOnlyCollection<Vehicle> Vehicles { get; } = vehicles;
    }
}
