using System.Threading.Tasks;

namespace GtMotive.Estimate.Microservice.Domain.Interfaces
{
    /// <summary>
    /// API del bus.
    /// </summary>
    public interface IBus
    {
        /// <summary>
        /// Envía un mensaje a la cola o tema asociado a este cliente.
        /// </summary>
        /// <param name="message">Mensaje.</param>
        /// <returns>Una tarea que se resuelve cuando la operación ha terminado.</returns>
        Task Send(object message);
    }
}
