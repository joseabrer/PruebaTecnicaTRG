using System.Threading.Tasks;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases
{
    /// <summary>
    /// Interfaz del manejador de un caso de uso.
    /// </summary>
    /// <typeparam name="TUseCaseInput">Tipo del mensaje de entrada.</typeparam>
    public interface IUseCase<in TUseCaseInput>
        where TUseCaseInput : IUseCaseInput
    {
        /// <summary>
        /// Ejecuta el caso de uso.
        /// </summary>
        /// <param name="input">Mensaje de entrada.</param>
        /// <returns>Tarea.</returns>
        Task Execute(TUseCaseInput input);
    }
}
