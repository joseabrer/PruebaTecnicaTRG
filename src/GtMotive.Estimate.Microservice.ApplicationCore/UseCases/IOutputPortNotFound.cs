namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases
{
    /// <summary>
    /// Interfaz que define el puerto de salida de recurso no encontrado.
    /// </summary>
    public interface IOutputPortNotFound
    {
        /// <summary>
        /// Informa de que no se encontró el recurso.
        /// </summary>
        /// <param name="message">Descripción en texto.</param>
        void NotFoundHandle(string message);
    }
}
