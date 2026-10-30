using System;
using System.ComponentModel.DataAnnotations;
using GtMotive.Estimate.Microservice.Api.Respuestas;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.RentVehicle
{
    /// <summary>
    /// Alquila un vehículo a una persona que no tiene otro vehículo alquilado.
    /// </summary>
    public sealed class AlquilarVehiculoSolicitud : IRequest<IWebApiPresenter>
    {
        /// <summary>
        /// Obtiene o establece el identificador del vehículo que se alquila.
        /// </summary>
        [Required(ErrorMessage = Mensajes.Obligatorio)]
        public Guid? VehiculoId { get; set; }

        /// <summary>
        /// Obtiene o establece la persona que alquila el vehículo (por ejemplo un número de DNI o un email).
        /// </summary>
        [Required(ErrorMessage = Mensajes.Obligatorio)]
        [StringLength(100, ErrorMessage = Mensajes.LongitudMaxima)]
        public string ClienteId { get; set; }
    }
}
