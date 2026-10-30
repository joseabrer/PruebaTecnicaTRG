namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Un vehículo con la misma matrícula ya forma parte de la flota.
    /// </summary>
    public sealed class LicensePlateAlreadyRegisteredException : DomainException
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="LicensePlateAlreadyRegisteredException"/>.
        /// </summary>
        /// <param name="message">Mensaje.</param>
        public LicensePlateAlreadyRegisteredException(string message)
            : base(message)
        {
        }
    }
}
