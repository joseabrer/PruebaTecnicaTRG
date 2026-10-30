using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.CreateVehicle
{
    /// <summary>
    /// Añade un vehículo nuevo a la flota. Las reglas de admisión (antigüedad máxima, fecha de fabricación válida) las aplica el dominio.
    /// </summary>
    public sealed class CreateVehicleUseCase(
        IVehicleRepository vehicleRepository,
        TimeProvider timeProvider,
        ICreateVehicleOutputPort outputPort) : IUseCase<CreateVehicleInput>
    {
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
        private readonly TimeProvider _timeProvider = timeProvider;
        private readonly ICreateVehicleOutputPort _outputPort = outputPort;

        /// <inheritdoc />
        public async Task Execute(CreateVehicleInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
            var vehicle = Vehicle.Register(input.LicensePlate, input.Brand, input.Model, input.ManufactureDate, today);

            try
            {
                await _vehicleRepository.Add(vehicle);
            }
            catch (LicensePlateAlreadyRegisteredException ex)
            {
                _outputPort.ConflictHandle(ex.Message);
                return;
            }

            _outputPort.StandardHandle(new CreateVehicleOutput(vehicle));
        }
    }
}
