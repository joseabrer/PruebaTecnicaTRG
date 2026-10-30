namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.CreateVehicle
{
    /// <summary>
    /// Puerto de salida del caso de uso Crear vehículo: el vehículo se añadió o la matrícula ya está en uso.
    /// </summary>
    public interface ICreateVehicleOutputPort : IOutputPortStandard<CreateVehicleOutput>, IOutputPortConflict
    {
    }
}
