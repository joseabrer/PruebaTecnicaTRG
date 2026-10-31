using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Rentals;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle
{
    /// <summary>
    /// Alquila un vehículo disponible, y que no supere la antigüedad máxima de la flota, a una persona que no tiene
    /// otro vehículo alquilado.
    /// No hay transacciones entre varios documentos, así que la coherencia entre el vehículo y el alquiler
    /// se apoya en actualizaciones atómicas en los repositorios más un paso de compensación.
    /// </summary>
    public sealed class RentVehicleUseCase(
        IVehicleRepository vehicleRepository,
        IRentalRepository rentalRepository,
        TimeProvider timeProvider,
        IRentVehicleOutputPort outputPort) : IUseCase<RentVehicleInput>
    {
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
        private readonly IRentalRepository _rentalRepository = rentalRepository;
        private readonly TimeProvider _timeProvider = timeProvider;
        private readonly IRentVehicleOutputPort _outputPort = outputPort;

        /// <inheritdoc />
        public async Task Execute(RentVehicleInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            try
            {
                if (await _rentalRepository.ExistsActiveByRenter(input.RenterId))
                {
                    _outputPort.ConflictHandle($"El cliente {input.RenterId.Value} ya tiene un vehículo alquilado.");
                    return;
                }

                var vehicle = await _vehicleRepository.GetById(input.VehicleId);
                var today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().UtcDateTime);
                vehicle.Rent(today);

                // Se reclama primero el vehículo: la actualización es atómica, así que solo una petición puede quedárselo.
                await _vehicleRepository.Update(vehicle);

                var rental = await StartRental(vehicle, input.RenterId);

                _outputPort.StandardHandle(new RentVehicleOutput(rental, vehicle));
            }
            catch (VehicleNotFoundException ex)
            {
                _outputPort.NotFoundHandle(ex.Message);
            }
            catch (VehicleNotAvailableException ex)
            {
                _outputPort.ConflictHandle(ex.Message);
            }
            catch (RenterAlreadyHasActiveRentalException ex)
            {
                _outputPort.ConflictHandle(ex.Message);
            }
            catch (ConcurrentModificationException ex)
            {
                _outputPort.ConflictHandle(ex.Message);
            }
        }

        private async Task<Rental> StartRental(Vehicle vehicle, RenterId renterId)
        {
            var rental = Rental.Start(vehicle.Id, renterId, _timeProvider.GetUtcNow());

            try
            {
                await _rentalRepository.Add(rental);
            }
            catch (Exception)
            {
                // Compensación: no se pudo guardar el alquiler, así que se libera el vehículo que se había reclamado.
                vehicle.Return();
                await _vehicleRepository.Update(vehicle);
                throw;
            }

            return rental;
        }
    }
}
