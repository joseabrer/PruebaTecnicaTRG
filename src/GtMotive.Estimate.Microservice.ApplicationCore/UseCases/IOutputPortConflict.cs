namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases
{
    /// <summary>
    /// Interfaz que define el puerto de salida de conflicto.
    /// </summary>
    public interface IOutputPortConflict
    {
        /// <summary>
        /// Informa de que la petición entra en conflicto con el estado actual de los recursos.
        /// </summary>
        /// <param name="message">Descripción en texto.</param>
        void ConflictHandle(string message);
    }
}
