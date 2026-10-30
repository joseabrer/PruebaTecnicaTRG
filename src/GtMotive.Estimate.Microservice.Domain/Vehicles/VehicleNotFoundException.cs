namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// El vehículo solicitado no existe.
    /// </summary>
    public sealed class VehicleNotFoundException : DomainException
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="VehicleNotFoundException"/>.
        /// </summary>
        /// <param name="message">Mensaje.</param>
        public VehicleNotFoundException(string message)
            : base(message)
        {
        }
    }
}
