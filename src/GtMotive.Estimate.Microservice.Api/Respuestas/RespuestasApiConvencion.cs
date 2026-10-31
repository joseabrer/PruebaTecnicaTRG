using System;
using System.Linq;
using GtMotive.Estimate.Microservice.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace GtMotive.Estimate.Microservice.Api.Respuestas
{
    /// <summary>
    /// Convierte cada <see cref="RespuestasApiAttribute"/> en los metadatos ProducesResponseType que MVC y Swagger
    /// entienden. Se ejecuta una sola vez, al arrancar la aplicación, para cada acción que tiene el atributo. Todas las
    /// respuestas se publican dentro del sobre <see cref="RespuestaApi{T}"/>, las de éxito y las de error.
    /// </summary>
    internal sealed class RespuestasApiConvencion : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            ArgumentNullException.ThrowIfNull(application);

            foreach (var accion in application.Controllers.SelectMany(controlador => controlador.Actions))
            {
                DeclararRespuestas(accion);
            }
        }

        private static void DeclararRespuestas(ActionModel accion)
        {
            var atributo = accion.Attributes.OfType<RespuestasApiAttribute>().FirstOrDefault();

            if (atributo is null)
            {
                return;
            }

            var tipoDelSobre = typeof(RespuestaApi<>).MakeGenericType(atributo.TipoRespuesta);
            accion.Filters.Add(new ProducesResponseTypeAttribute(tipoDelSobre, atributo.CodigoExito));

            if (atributo.SolicitudInvalida)
            {
                AgregarError(accion, StatusCodes.Status400BadRequest);
            }

            if (atributo.NoEncontrado)
            {
                AgregarError(accion, StatusCodes.Status404NotFound);
            }

            if (atributo.Conflicto)
            {
                AgregarError(accion, StatusCodes.Status409Conflict);
            }
        }

        private static void AgregarError(ActionModel accion, int codigo)
        {
            accion.Filters.Add(new ProducesResponseTypeAttribute(typeof(RespuestaApi<object>), codigo));
        }
    }
}
