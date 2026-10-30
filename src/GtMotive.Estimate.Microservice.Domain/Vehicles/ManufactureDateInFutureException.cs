namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// La fecha de fabricación de un vehículo no puede estar en el futuro.
    /// </summary>
    public sealed class ManufactureDateInFutureException : DomainException
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ManufactureDateInFutureException"/>.
        /// </summary>
        /// <param name="message">Mensaje.</param>
        public ManufactureDateInFutureException(string message)
            : base(message)
        {
        }
    }
}
