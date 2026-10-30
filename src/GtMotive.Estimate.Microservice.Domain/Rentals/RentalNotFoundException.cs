namespace GtMotive.Estimate.Microservice.Domain.Rentals
{
    /// <summary>
    /// El alquiler solicitado no existe.
    /// </summary>
    public sealed class RentalNotFoundException : DomainException
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="RentalNotFoundException"/>.
        /// </summary>
        /// <param name="message">Mensaje.</param>
        public RentalNotFoundException(string message)
            : base(message)
        {
        }
    }
}
