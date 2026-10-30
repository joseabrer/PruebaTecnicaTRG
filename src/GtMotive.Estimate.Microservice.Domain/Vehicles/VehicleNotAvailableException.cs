namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// El vehículo no se puede alquilar porque no está disponible.
    /// </summary>
    public sealed class VehicleNotAvailableException : DomainException
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="VehicleNotAvailableException"/>.
        /// </summary>
        /// <param name="message">Mensaje.</param>
        public VehicleNotAvailableException(string message)
            : base(message)
        {
        }
    }
}
