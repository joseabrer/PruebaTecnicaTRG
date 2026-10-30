using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.Domain.Interfaces
{
    /// <summary>
    /// Puerto de persistencia del agregado <see cref="Vehicle"/>.
    /// </summary>
    public interface IVehicleRepository
    {
        /// <summary>
        /// Añade un vehículo nuevo a la flota.
        /// </summary>
        /// <param name="vehicle">Vehículo que se añade.</param>
        /// <returns>Tarea.</returns>
        /// <exception cref="LicensePlateAlreadyRegisteredException">Ya existe un vehículo con la misma matrícula.</exception>
        Task Add(Vehicle vehicle);

        /// <summary>
        /// Obtiene un vehículo por su identificador.
        /// </summary>
        /// <param name="id">Identificador del vehículo.</param>
        /// <returns>El vehículo.</returns>
        /// <exception cref="VehicleNotFoundException">El vehículo no existe.</exception>
        Task<Vehicle> GetById(Guid id);

        /// <summary>
        /// Lista los vehículos que se pueden alquilar ahora mismo: los disponibles fabricados en la fecha indicada o después.
        /// </summary>
        /// <param name="oldestManufactureDate">Fecha de fabricación más antigua que se admite.</param>
        /// <returns>Los vehículos disponibles.</returns>
        Task<IReadOnlyCollection<Vehicle>> ListAvailable(DateOnly oldestManufactureDate);

        /// <summary>
        /// Guarda los cambios hechos en un vehículo existente. La actualización es atómica: solo se aplica
        /// si otra petición no ha modificado el vehículo guardado desde que se cargó.
        /// </summary>
        /// <param name="vehicle">Vehículo que se actualiza.</param>
        /// <returns>Tarea.</returns>
        /// <exception cref="ConcurrentModificationException">Otra petición modificó el vehículo.</exception>
        Task Update(Vehicle vehicle);
    }
}
