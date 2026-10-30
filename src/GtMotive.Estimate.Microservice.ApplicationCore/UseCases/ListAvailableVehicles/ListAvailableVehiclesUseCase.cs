using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ListAvailableVehicles
{
    /// <summary>
    /// Lista los vehículos de la flota que se pueden alquilar ahora mismo: los disponibles y que no superan la
    /// antigüedad máxima. Un vehículo que envejece más allá de ese límite deja de ofrecerse.
    /// </summary>
    public sealed class ListAvailableVehiclesUseCase(
        IVehicleRepository vehicleRepository,
        TimeProvider timeProvider,
        IListAvailableVehiclesOutputPort outputPort) : IUseCase<ListAvailableVehiclesInput>
    {
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
        private readonly TimeProvider _timeProvider = timeProvider;
        private readonly IListAvailableVehiclesOutputPort _outputPort = outputPort;

        /// <inheritdoc />
        public async Task Execute(ListAvailableVehiclesInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
            var vehicles = await _vehicleRepository.ListAvailable(Vehicle.OldestAllowedManufactureDate(today));

            _outputPort.StandardHandle(new ListAvailableVehiclesOutput(vehicles));
        }
    }
}
