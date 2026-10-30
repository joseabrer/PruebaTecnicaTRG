using System;
using System.Threading.Tasks;
using FluentAssertions;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ListAvailableVehicles;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using Moq;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.ApplicationCore
{
    /// <summary>
    /// Tests unitarios del caso de uso Listar vehículos disponibles, centrados en que solo se ofrecen los vehículos
    /// que no superan la antigüedad máxima. El reloj, el repositorio y el puerto de salida son mocks.
    /// </summary>
    public sealed class ListAvailableVehiclesUseCaseTests
    {
        private static readonly DateTimeOffset Now = new(2026, 10, 30, 10, 0, 0, TimeSpan.Zero);

        private readonly Mock<IVehicleRepository> _vehicles = new();
        private readonly Mock<IListAvailableVehiclesOutputPort> _outputPort = new();
        private readonly ListAvailableVehiclesUseCase _useCase;

        public ListAvailableVehiclesUseCaseTests()
        {
            var timeProvider = new Mock<TimeProvider>();
            timeProvider.Setup(x => x.GetUtcNow()).Returns(Now);

            _useCase = new ListAvailableVehiclesUseCase(_vehicles.Object, timeProvider.Object, _outputPort.Object);
        }

        [Fact]
        public async Task ExecuteOnlyAsksForVehiclesManufacturedWithinTheLastFiveYears()
        {
            _vehicles.Setup(x => x.ListAvailable(It.IsAny<DateOnly>())).ReturnsAsync([]);

            await _useCase.Execute(new ListAvailableVehiclesInput());

            // Today is 2026-10-30, so a vehicle manufactured on 2021-10-30 is the oldest one that still counts.
            _vehicles.Verify(x => x.ListAvailable(new DateOnly(2021, 10, 30)), Times.Once);
        }

        [Fact]
        public async Task ExecuteHandsTheVehiclesFoundToTheOutputPort()
        {
            var vehicle = Vehicle.Restore(Guid.NewGuid(), LicensePlate.Create("ab-123"), "Seat", "Ibiza", new DateOnly(2025, 1, 1), VehicleStatus.Available);
            ListAvailableVehiclesOutput output = null;
            _vehicles.Setup(x => x.ListAvailable(It.IsAny<DateOnly>())).ReturnsAsync([vehicle]);
            _outputPort.Setup(x => x.StandardHandle(It.IsAny<ListAvailableVehiclesOutput>())).Callback<ListAvailableVehiclesOutput>(o => output = o);

            await _useCase.Execute(new ListAvailableVehiclesInput());

            output.Vehicles.Should().ContainSingle().Which.Should().BeSameAs(vehicle);
        }
    }
}
