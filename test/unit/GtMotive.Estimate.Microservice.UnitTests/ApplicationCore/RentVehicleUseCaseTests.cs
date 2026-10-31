using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Rentals;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using Moq;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore
{
    /// <summary>
    /// Tests unitarios del caso de uso Alquilar vehículo. Sus dependencias (repositorios, reloj y puerto de salida)
    /// se sustituyen por mocks, así que solo se ejercita la lógica del caso de uso.
    /// </summary>
    public sealed class RentVehicleUseCaseTests
    {
        private static readonly DateTimeOffset Now = new(2026, 10, 30, 10, 0, 0, TimeSpan.Zero);

        private readonly Mock<IVehicleRepository> _vehicles = new();
        private readonly Mock<IRentalRepository> _rentals = new();
        private readonly Mock<IRentVehicleOutputPort> _outputPort = new();
        private readonly RentVehicleUseCase _useCase;

        public RentVehicleUseCaseTests()
        {
            var timeProvider = new Mock<TimeProvider>();
            timeProvider.Setup(x => x.GetUtcNow()).Returns(Now);

            _useCase = new RentVehicleUseCase(_vehicles.Object, _rentals.Object, timeProvider.Object, _outputPort.Object);
        }

        [Fact]
        public async Task ExecuteRentsTheVehicleWhenItIsAvailableAndThePersonHasNoOtherRental()
        {
            var vehicle = NewVehicle(VehicleStatus.Available);
            var renter = RenterId.Create("ana");
            _vehicles.Setup(x => x.GetById(vehicle.Id)).ReturnsAsync(vehicle);

            await _useCase.Execute(new RentVehicleInput(vehicle.Id, renter));

            vehicle.Status.Should().Be(VehicleStatus.Rented);
            _vehicles.Verify(x => x.Update(vehicle), Times.Once);
            _rentals.Verify(
                x => x.Add(It.Is<Rental>(r => r.VehicleId == vehicle.Id && r.RenterId == renter && r.StartedAt == Now && r.IsActive)),
                Times.Once);
            _outputPort.Verify(x => x.StandardHandle(It.IsAny<RentVehicleOutput>()), Times.Once);
            _outputPort.Verify(x => x.ConflictHandle(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteReportsConflictAndDoesNotTouchTheVehicleWhenThePersonAlreadyHasARental()
        {
            var renter = RenterId.Create("ana");
            _rentals.Setup(x => x.ExistsActiveByRenter(renter)).ReturnsAsync(true);

            await _useCase.Execute(new RentVehicleInput(Guid.NewGuid(), renter));

            _outputPort.Verify(x => x.ConflictHandle(It.IsAny<string>()), Times.Once);
            _outputPort.Verify(x => x.StandardHandle(It.IsAny<RentVehicleOutput>()), Times.Never);
            _vehicles.Verify(x => x.GetById(It.IsAny<Guid>()), Times.Never);
            _vehicles.Verify(x => x.Update(It.IsAny<Vehicle>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteReportsNotFoundWhenTheVehicleDoesNotExist()
        {
            var vehicleId = Guid.NewGuid();
            _vehicles.Setup(x => x.GetById(vehicleId)).ThrowsAsync(new VehicleNotFoundException("not found"));

            await _useCase.Execute(new RentVehicleInput(vehicleId, RenterId.Create("ana")));

            _outputPort.Verify(x => x.NotFoundHandle("not found"), Times.Once);
            _rentals.Verify(x => x.Add(It.IsAny<Rental>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteReportsConflictWhenTheVehicleIsAlreadyRented()
        {
            var vehicle = NewVehicle(VehicleStatus.Rented);
            _vehicles.Setup(x => x.GetById(vehicle.Id)).ReturnsAsync(vehicle);

            await _useCase.Execute(new RentVehicleInput(vehicle.Id, RenterId.Create("ana")));

            _outputPort.Verify(x => x.ConflictHandle(It.IsAny<string>()), Times.Once);
            _vehicles.Verify(x => x.Update(It.IsAny<Vehicle>()), Times.Never);
            _rentals.Verify(x => x.Add(It.IsAny<Rental>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteReleasesTheVehicleWhenTheRentalCannotBeSaved()
        {
            var vehicle = NewVehicle(VehicleStatus.Available);
            var statusesWhenUpdated = new List<VehicleStatus>();
            _vehicles.Setup(x => x.GetById(vehicle.Id)).ReturnsAsync(vehicle);
            _vehicles.Setup(x => x.Update(It.IsAny<Vehicle>())).Callback<Vehicle>(v => statusesWhenUpdated.Add(v.Status)).Returns(Task.CompletedTask);
            _rentals.Setup(x => x.Add(It.IsAny<Rental>())).ThrowsAsync(new RenterAlreadyHasActiveRentalException("lost the race"));

            await _useCase.Execute(new RentVehicleInput(vehicle.Id, RenterId.Create("ana")));

            statusesWhenUpdated.Should().Equal(VehicleStatus.Rented, VehicleStatus.Available);
            _outputPort.Verify(x => x.ConflictHandle("lost the race"), Times.Once);
            _outputPort.Verify(x => x.StandardHandle(It.IsAny<RentVehicleOutput>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteReportsConflictWhenAnotherRequestTookTheVehicleFirst()
        {
            var vehicle = NewVehicle(VehicleStatus.Available);
            _vehicles.Setup(x => x.GetById(vehicle.Id)).ReturnsAsync(vehicle);
            _vehicles.Setup(x => x.Update(vehicle)).ThrowsAsync(new ConcurrentModificationException("changed by someone else"));

            await _useCase.Execute(new RentVehicleInput(vehicle.Id, RenterId.Create("ana")));

            _outputPort.Verify(x => x.ConflictHandle("changed by someone else"), Times.Once);
            _rentals.Verify(x => x.Add(It.IsAny<Rental>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteReportsConflictAndDoesNotClaimTheVehicleWhenItIsOlderThanTheMaximumAge()
        {
            // Cinco años y un día antes de Now: era válido cuando entró en la flota, pero ya no lo es.
            var vehicle = NewVehicle(VehicleStatus.Available, new DateOnly(2021, 10, 29));
            _vehicles.Setup(x => x.GetById(vehicle.Id)).ReturnsAsync(vehicle);

            await _useCase.Execute(new RentVehicleInput(vehicle.Id, RenterId.Create("ana")));

            vehicle.Status.Should().Be(VehicleStatus.Available);
            _outputPort.Verify(x => x.ConflictHandle(It.IsAny<string>()), Times.Once);
            _outputPort.Verify(x => x.StandardHandle(It.IsAny<RentVehicleOutput>()), Times.Never);
            _vehicles.Verify(x => x.Update(It.IsAny<Vehicle>()), Times.Never);
            _rentals.Verify(x => x.Add(It.IsAny<Rental>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteRentsAVehicleManufacturedExactlyFiveYearsAgo()
        {
            var vehicle = NewVehicle(VehicleStatus.Available, new DateOnly(2021, 10, 30));
            _vehicles.Setup(x => x.GetById(vehicle.Id)).ReturnsAsync(vehicle);

            await _useCase.Execute(new RentVehicleInput(vehicle.Id, RenterId.Create("ana")));

            vehicle.Status.Should().Be(VehicleStatus.Rented);
            _outputPort.Verify(x => x.StandardHandle(It.IsAny<RentVehicleOutput>()), Times.Once);
        }

        private static Vehicle NewVehicle(VehicleStatus status)
        {
            return NewVehicle(status, new DateOnly(2025, 1, 1));
        }

        private static Vehicle NewVehicle(VehicleStatus status, DateOnly manufactureDate)
        {
            return Vehicle.Restore(
                Guid.NewGuid(),
                LicensePlate.Create("ab-123"),
                "Seat",
                "Ibiza",
                manufactureDate,
                status);
        }
    }
}
