using System;

namespace GtMotive.Estimate.Microservice.Domain.Rentals
{
    /// <summary>
    /// Alquiler de un vehículo por una persona. Raíz de agregado: está activo desde que empieza hasta que se devuelve el vehículo.
    /// </summary>
    public sealed class Rental
    {
        private Rental(
            Guid id,
            Guid vehicleId,
            RenterId renterId,
            DateTimeOffset startedAt,
            DateTimeOffset? returnedAt)
        {
            Id = id;
            VehicleId = vehicleId;
            RenterId = renterId;
            StartedAt = startedAt;
            ReturnedAt = returnedAt;
        }

        /// <summary>
        /// Obtiene el identificador único del alquiler.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Obtiene el identificador del vehículo alquilado. Los otros agregados se referencian solo por identidad.
        /// </summary>
        public Guid VehicleId { get; }

        /// <summary>
        /// Obtiene la persona que alquiló el vehículo.
        /// </summary>
        public RenterId RenterId { get; }

        /// <summary>
        /// Obtiene el momento en que empezó el alquiler.
        /// </summary>
        public DateTimeOffset StartedAt { get; }

        /// <summary>
        /// Obtiene el momento en que se devolvió el vehículo, o null mientras el alquiler está activo.
        /// </summary>
        public DateTimeOffset? ReturnedAt { get; private set; }

        /// <summary>
        /// Obtiene un valor que indica si el vehículo aún no se ha devuelto.
        /// </summary>
        public bool IsActive => ReturnedAt is null;

        /// <summary>
        /// Inicia un alquiler nuevo y activo.
        /// </summary>
        /// <param name="vehicleId">Identificador del vehículo que se alquila.</param>
        /// <param name="renterId">Persona que alquila el vehículo.</param>
        /// <param name="now">Momento actual, proporcionado por quien llama.</param>
        /// <returns>El alquiler nuevo y activo.</returns>
        public static Rental Start(Guid vehicleId, RenterId renterId, DateTimeOffset now)
        {
            ArgumentNullException.ThrowIfNull(renterId);

            return new Rental(Guid.NewGuid(), vehicleId, renterId, now, null);
        }

        /// <summary>
        /// Reconstruye un alquiler a partir de su estado guardado.
        /// </summary>
        /// <param name="id">Identificador.</param>
        /// <param name="vehicleId">Identificador del vehículo alquilado.</param>
        /// <param name="renterId">Persona que alquiló el vehículo.</param>
        /// <param name="startedAt">Momento en que empezó el alquiler.</param>
        /// <param name="returnedAt">Momento en que se devolvió el vehículo, o null si sigue alquilado.</param>
        /// <returns>El alquiler reconstruido.</returns>
        public static Rental Restore(
            Guid id,
            Guid vehicleId,
            RenterId renterId,
            DateTimeOffset startedAt,
            DateTimeOffset? returnedAt)
        {
            ArgumentNullException.ThrowIfNull(renterId);

            return new Rental(id, vehicleId, renterId, startedAt, returnedAt);
        }

        /// <summary>
        /// Cierra el alquiler porque se ha devuelto el vehículo.
        /// </summary>
        /// <param name="now">Momento actual, proporcionado por quien llama.</param>
        public void Return(DateTimeOffset now)
        {
            if (!IsActive)
            {
                throw new RentalAlreadyReturnedException("El alquiler ya ha sido devuelto.");
            }

            ReturnedAt = now;
        }
    }
}
