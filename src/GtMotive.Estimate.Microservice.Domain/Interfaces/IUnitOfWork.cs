using System.Threading.Tasks;

namespace GtMotive.Estimate.Microservice.Domain.Interfaces
{
    /// <summary>
    /// Unidad de trabajo. Solo deben usarla los casos de uso.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Aplica todos los cambios de la base de datos.
        /// </summary>
        /// <returns>Número de filas afectadas.</returns>
        Task<int> Save();
    }
}
