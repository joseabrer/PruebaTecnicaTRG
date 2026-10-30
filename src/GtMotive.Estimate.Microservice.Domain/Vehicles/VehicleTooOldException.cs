namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Un vehículo más antiguo que la antigüedad máxima permitida no se puede añadir a la flota.
    /// </summary>
    public sealed class VehicleTooOldException : DomainException
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="VehicleTooOldException"/>.
        /// </summary>
        /// <param name="message">Mensaje.</param>
        public VehicleTooOldException(string message)
            : base(message)
        {
        }
    }
}
