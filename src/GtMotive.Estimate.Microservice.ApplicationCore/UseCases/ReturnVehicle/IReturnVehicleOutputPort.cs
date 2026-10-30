namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ReturnVehicle
{
    /// <summary>
    /// Puerto de salida del caso de uso Devolver vehículo: el vehículo se devolvió, el alquiler no existe
    /// o la petición entra en conflicto con el estado actual (alquiler ya devuelto).
    /// </summary>
    public interface IReturnVehicleOutputPort : IOutputPortStandard<ReturnVehicleOutput>, IOutputPortNotFound, IOutputPortConflict
    {
    }
}
