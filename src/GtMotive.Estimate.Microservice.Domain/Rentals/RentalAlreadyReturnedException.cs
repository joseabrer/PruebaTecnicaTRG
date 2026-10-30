namespace GtMotive.Estimate.Microservice.Domain.Rentals
{
    /// <summary>
    /// El alquiler no se puede devolver porque ya se devolvió.
    /// </summary>
    public sealed class RentalAlreadyReturnedException : DomainException
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="RentalAlreadyReturnedException"/>.
        /// </summary>
        /// <param name="message">Mensaje.</param>
        public RentalAlreadyReturnedException(string message)
            : base(message)
        {
        }
    }
}
