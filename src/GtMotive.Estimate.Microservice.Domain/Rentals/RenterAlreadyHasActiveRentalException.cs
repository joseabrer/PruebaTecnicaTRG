namespace GtMotive.Estimate.Microservice.Domain.Rentals
{
    /// <summary>
    /// Una persona no puede alquilar más de un vehículo a la vez.
    /// </summary>
    public sealed class RenterAlreadyHasActiveRentalException : DomainException
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="RenterAlreadyHasActiveRentalException"/>.
        /// </summary>
        /// <param name="message">Mensaje.</param>
        public RenterAlreadyHasActiveRentalException(string message)
            : base(message)
        {
        }
    }
}
