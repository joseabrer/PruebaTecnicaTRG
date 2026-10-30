using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Rentals;

namespace GtMotive.Estimate.Microservice.Domain.Interfaces
{
    /// <summary>
    /// Puerto de persistencia del agregado <see cref="Rental"/>.
    /// </summary>
    public interface IRentalRepository
    {
        /// <summary>
        /// Añade un alquiler nuevo.
        /// </summary>
        /// <param name="rental">Alquiler que se añade.</param>
        /// <returns>Tarea.</returns>
        /// <exception cref="RenterAlreadyHasActiveRentalException">La persona ya tiene un alquiler activo.</exception>
        Task Add(Rental rental);

        /// <summary>
        /// Obtiene un alquiler por su identificador.
        /// </summary>
        /// <param name="id">Identificador del alquiler.</param>
        /// <returns>El alquiler.</returns>
        /// <exception cref="RentalNotFoundException">El alquiler no existe.</exception>
        Task<Rental> GetById(Guid id);

        /// <summary>
        /// Comprueba si una persona tiene ahora mismo un vehículo alquilado.
        /// </summary>
        /// <param name="renterId">Persona que se comprueba.</param>
        /// <returns>True si la persona tiene un alquiler activo.</returns>
        Task<bool> ExistsActiveByRenter(RenterId renterId);

        /// <summary>
        /// Guarda los cambios hechos en un alquiler existente. La actualización es atómica: solo se aplica
        /// si otra petición no ha modificado el alquiler guardado desde que se cargó.
        /// </summary>
        /// <param name="rental">Alquiler que se actualiza.</param>
        /// <returns>Tarea.</returns>
        /// <exception cref="ConcurrentModificationException">Otra petición modificó el alquiler.</exception>
        Task Update(Rental rental);
    }
}
