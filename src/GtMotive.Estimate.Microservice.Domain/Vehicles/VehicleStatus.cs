namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Disponibilidad de un vehículo de la flota.
    /// </summary>
    public enum VehicleStatus
    {
        /// <summary>
        /// El vehículo se puede alquilar.
        /// </summary>
        Available,

        /// <summary>
        /// El vehículo está alquilado actualmente.
        /// </summary>
        Rented
    }
}
