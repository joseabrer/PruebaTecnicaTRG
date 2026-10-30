namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases
{
    /// <summary>
    /// Interfaz que define el puerto de salida estándar.
    /// </summary>
    /// <typeparam name="TUseCaseOutput">Tipo del dto de respuesta del caso de uso.</typeparam>
    public interface IOutputPortStandard<in TUseCaseOutput>
        where TUseCaseOutput : IUseCaseOutput
    {
        /// <summary>
        /// Escribe en la salida estándar.
        /// </summary>
        /// <param name="response">El mensaje del puerto de salida.</param>
        void StandardHandle(TUseCaseOutput response);
    }
}
