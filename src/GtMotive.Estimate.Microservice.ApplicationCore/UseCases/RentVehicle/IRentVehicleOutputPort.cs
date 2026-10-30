namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle
{
    /// <summary>
    /// Puerto de salida del caso de uso Alquilar vehículo: el vehículo se alquiló, no existe
    /// o la petición entra en conflicto con el estado actual (vehículo ocupado, persona que ya tiene un alquiler).
    /// </summary>
    public interface IRentVehicleOutputPort : IOutputPortStandard<RentVehicleOutput>, IOutputPortNotFound, IOutputPortConflict
    {
    }
}
