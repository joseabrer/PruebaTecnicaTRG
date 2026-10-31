using System;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.CreateVehicle;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using Moq;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore
{
    /// <summary>
    /// Tests unitarios del caso de uso Crear vehículo, centrados en la regla de admisión de la flota:
    /// ningún vehículo fabricado hace más de 5 años. El reloj es un mock, así que las fechas son deterministas.
    /// </summary>
    public sealed class CreateVehicleUseCaseTests
    {
        private static readonly DateTimeOffset Now = new(2026, 10, 30, 10, 0, 0, TimeSpan.Zero);

        private readonly Mock<IVehicleRepository> _vehicles = new();
        private readonly Mock<ICreateVehicleOutputPort> _outputPort = new();
        private readonly CreateVehicleUseCase _useCase;

        public CreateVehicleUseCaseTests()
        {
            var timeProvider = new Mock<TimeProvider>();
            timeProvider.Setup(x => x.GetUtcNow()).Returns(Now);

            _useCase = new CreateVehicleUseCase(_vehicles.Object, timeProvider.Object, _outputPort.Object);
        }

        [Fact]
        public async Task ExecuteAddsTheVehicleWhenItIsExactlyFiveYearsOld()
        {
            await _useCase.Execute(NewInput(new DateOnly(2021, 10, 30)));

            _vehicles.Verify(x => x.Add(It.Is<Vehicle>(v => v.Status == VehicleStatus.Available)), Times.Once);
            _outputPort.Verify(x => x.StandardHandle(It.IsAny<CreateVehicleOutput>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteRejectsTheVehicleWhenItIsOlderThanFiveYears()
        {
            var act = () => _useCase.Execute(NewInput(new DateOnly(2021, 10, 29)));

            await act.Should().ThrowAsync<VehicleTooOldException>();
            _vehicles.Verify(x => x.Add(It.IsAny<Vehicle>()), Times.Never);
            _outputPort.Verify(x => x.StandardHandle(It.IsAny<CreateVehicleOutput>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteRejectsTheVehicleWhenTheManufactureDateIsInTheFuture()
        {
            var act = () => _useCase.Execute(NewInput(new DateOnly(2026, 12, 31)));

            await act.Should().ThrowAsync<ManufactureDateInFutureException>();
            _vehicles.Verify(x => x.Add(It.IsAny<Vehicle>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteReportsConflictWhenTheLicensePlateIsAlreadyInTheFleet()
        {
            _vehicles.Setup(x => x.Add(It.IsAny<Vehicle>())).ThrowsAsync(new LicensePlateAlreadyRegisteredException("duplicated"));

            await _useCase.Execute(NewInput(new DateOnly(2025, 1, 1)));

            _outputPort.Verify(x => x.ConflictHandle("duplicated"), Times.Once);
            _outputPort.Verify(x => x.StandardHandle(It.IsAny<CreateVehicleOutput>()), Times.Never);
        }

        private static CreateVehicleInput NewInput(DateOnly manufactureDate)
        {
            return new CreateVehicleInput(LicensePlate.Create("ab-123"), "Seat", "Ibiza", manufactureDate);
        }
    }
}
