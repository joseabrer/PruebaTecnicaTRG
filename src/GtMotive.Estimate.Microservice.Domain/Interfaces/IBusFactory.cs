using System;

namespace GtMotive.Estimate.Microservice.Domain.Interfaces
{
    /// <summary>
    /// Factoría de buses.
    /// </summary>
    public interface IBusFactory
    {
        /// <summary>
        /// Obtiene un cliente de bus para un tipo de evento.
        /// </summary>
        /// <param name="eventType">Tipo de evento.</param>
        /// <returns>Cliente de bus.</returns>
        IBus GetClient(Type eventType);
    }
}
