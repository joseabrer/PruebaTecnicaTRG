using System;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.CreateVehicle
{
    /// <summary>
    /// Mensaje de entrada del caso de uso Crear vehículo.
    /// </summary>
    public sealed class CreateVehicleInput(
        LicensePlate licensePlate,
        string brand,
        string model,
        DateOnly manufactureDate) : IUseCaseInput
    {
        /// <summary>
        /// Obtiene la matrícula del nuevo vehículo.
        /// </summary>
        public LicensePlate LicensePlate { get; } = licensePlate;

        /// <summary>
        /// Obtiene la marca del nuevo vehículo.
        /// </summary>
        public string Brand { get; } = brand;

        /// <summary>
        /// Obtiene el modelo del nuevo vehículo.
        /// </summary>
        public string Model { get; } = model;

        /// <summary>
        /// Obtiene la fecha de fabricación del nuevo vehículo.
        /// </summary>
        public DateOnly ManufactureDate { get; } = manufactureDate;
    }
}
