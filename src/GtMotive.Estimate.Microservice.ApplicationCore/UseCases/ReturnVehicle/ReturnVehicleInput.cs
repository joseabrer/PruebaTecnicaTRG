using System;

namespace GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ReturnVehicle
{
    /// <summary>
    /// Mensaje de entrada del caso de uso Devolver vehículo.
    /// </summary>
    public sealed class ReturnVehicleInput(Guid rentalId) : IUseCaseInput
    {
        /// <summary>
        /// Obtiene el identificador del alquiler que se cierra.
        /// </summary>
        public Guid RentalId { get; } = rentalId;
    }
}
