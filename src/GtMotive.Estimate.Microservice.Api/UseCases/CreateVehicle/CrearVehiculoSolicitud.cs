using System;
using System.ComponentModel.DataAnnotations;
using GtMotive.Estimate.Microservice.Api.Respuestas;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.CreateVehicle
{
    /// <summary>
    /// Añade un vehículo a la flota.
    /// </summary>
    public sealed class CrearVehiculoSolicitud : IRequest<IWebApiPresenter>
    {
        /// <summary>
        /// Obtiene o establece la matrícula. Debe ser única en la flota.
        /// </summary>
        [Required(ErrorMessage = Mensajes.Obligatorio)]
        [StringLength(15, ErrorMessage = Mensajes.LongitudMaxima)]
        public string Matricula { get; set; }

        /// <summary>
        /// Obtiene o establece la marca.
        /// </summary>
        [Required(ErrorMessage = Mensajes.Obligatorio)]
        [StringLength(100, ErrorMessage = Mensajes.LongitudMaxima)]
        public string Marca { get; set; }

        /// <summary>
        /// Obtiene o establece el modelo.
        /// </summary>
        [Required(ErrorMessage = Mensajes.Obligatorio)]
        [StringLength(100, ErrorMessage = Mensajes.LongitudMaxima)]
        public string Modelo { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de fabricación (yyyy-MM-dd). La flota solo admite vehículos de hasta 5 años.
        /// </summary>
        [Required(ErrorMessage = Mensajes.Obligatorio)]
        public DateOnly? FechaFabricacion { get; set; }
    }
}
