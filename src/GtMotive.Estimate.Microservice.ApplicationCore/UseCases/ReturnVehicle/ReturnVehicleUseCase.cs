using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Rentals;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ReturnVehicle
{
    /// <summary>
    /// Cierra un alquiler y hace que su vehículo vuelva a estar disponible.
    /// </summary>
    public sealed class ReturnVehicleUseCase(
        IVehicleRepository vehicleRepository,
        IRentalRepository rentalRepository,
        TimeProvider timeProvider,
        IReturnVehicleOutputPort outputPort) : IUseCase<ReturnVehicleInput>
    {
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
        private readonly IRentalRepository _rentalRepository = rentalRepository;
        private readonly TimeProvider _timeProvider = timeProvider;
        private readonly IReturnVehicleOutputPort _outputPort = outputPort;

        /// <inheritdoc />
        public async Task Execute(ReturnVehicleInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            try
            {
                var rental = await _rentalRepository.GetById(input.RentalId);
                var vehicle = await _vehicleRepository.GetById(rental.VehicleId);

                rental.Return(_timeProvider.GetUtcNow());
                vehicle.Return();

                // Close the rental first: if something fails afterwards the vehicle stays unavailable,
                // which is safer than offering it again while the rental is still open.
                await _rentalRepository.Update(rental);
                await _vehicleRepository.Update(vehicle);

                _outputPort.StandardHandle(new ReturnVehicleOutput(rental, vehicle));
            }
            catch (RentalNotFoundException ex)
            {
                _outputPort.NotFoundHandle(ex.Message);
            }
            catch (VehicleNotFoundException ex)
            {
                _outputPort.NotFoundHandle(ex.Message);
            }
            catch (RentalAlreadyReturnedException ex)
            {
                _outputPort.ConflictHandle(ex.Message);
            }
            catch (VehicleNotRentedException ex)
            {
                _outputPort.ConflictHandle(ex.Message);
            }
            catch (ConcurrentModificationException ex)
            {
                _outputPort.ConflictHandle(ex.Message);
            }
        }
    }
}
