using System;
using System.ComponentModel.DataAnnotations;
using GtMotive.Estimate.Microservice.Domain.Rentals;

namespace GtMotive.Estimate.Microservice.Api.Models
{
    /// <summary>
    /// El alquiler de un vehículo por una persona, tal como lo ven los clientes de la API.
    /// </summary>
    public sealed class AlquilerRespuesta
    {
        public AlquilerRespuesta(Rental alquiler)
        {
            ArgumentNullException.ThrowIfNull(alquiler);

            Id = alquiler.Id;
            VehiculoId = alquiler.VehicleId;
            ClienteId = alquiler.RenterId.Value;
            FechaInicio = alquiler.StartedAt;
            FechaDevolucion = alquiler.ReturnedAt;
            Activo = alquiler.IsActive;
        }

        /// <summary>
        /// Obtiene el identificador del alquiler. Es el que hace falta para devolver el vehículo.
        /// </summary>
        [Required]
        public Guid Id { get; }

        /// <summary>
        /// Obtiene el identificador del vehículo alquilado.
        /// </summary>
        [Required]
        public Guid VehiculoId { get; }

        /// <summary>
        /// Obtiene la persona que alquiló el vehículo, normalizada a mayúsculas.
        /// </summary>
        [Required]
        public string ClienteId { get; }

        /// <summary>
        /// Obtiene el momento en que empezó el alquiler.
        /// </summary>
        [Required]
        public DateTimeOffset FechaInicio { get; }

        /// <summary>
        /// Obtiene el momento en que se devolvió el vehículo, o null mientras el alquiler está activo.
        /// </summary>
        public DateTimeOffset? FechaDevolucion { get; }

        /// <summary>
        /// Obtiene un valor que indica si el vehículo aún no se ha devuelto.
        /// </summary>
        [Required]
        public bool Activo { get; }
    }
}
