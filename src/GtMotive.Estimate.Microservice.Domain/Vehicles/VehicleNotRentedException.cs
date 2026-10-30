namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// El vehículo no se puede devolver porque no está alquilado.
    /// </summary>
    public sealed class VehicleNotRentedException : DomainException
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="VehicleNotRentedException"/>.
        /// </summary>
        /// <param name="message">Mensaje.</param>
        public VehicleNotRentedException(string message)
            : base(message)
        {
        }
    }
}
