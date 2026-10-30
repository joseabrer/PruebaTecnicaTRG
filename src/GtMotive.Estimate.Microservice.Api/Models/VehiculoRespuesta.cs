using System;
using System.ComponentModel.DataAnnotations;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.Api.Models
{
    /// <summary>
    /// Un vehículo de la flota, tal como lo ven los clientes de la API.
    /// </summary>
    public sealed class VehiculoRespuesta
    {
        public VehiculoRespuesta(Vehicle vehiculo)
        {
            ArgumentNullException.ThrowIfNull(vehiculo);

            Id = vehiculo.Id;
            Matricula = vehiculo.LicensePlate.Value;
            Marca = vehiculo.Brand;
            Modelo = vehiculo.Model;
            FechaFabricacion = vehiculo.ManufactureDate;
        }

        /// <summary>
        /// Obtiene el identificador del vehículo.
        /// </summary>
        [Required]
        public Guid Id { get; }

        /// <summary>
        /// Obtiene la matrícula, normalizada a mayúsculas.
        /// </summary>
        [Required]
        public string Matricula { get; }

        /// <summary>
        /// Obtiene la marca.
        /// </summary>
        [Required]
        public string Marca { get; }

        /// <summary>
        /// Obtiene el modelo.
        /// </summary>
        [Required]
        public string Modelo { get; }

        /// <summary>
        /// Obtiene la fecha de fabricación.
        /// </summary>
        [Required]
        public DateOnly FechaFabricacion { get; }
    }
}
