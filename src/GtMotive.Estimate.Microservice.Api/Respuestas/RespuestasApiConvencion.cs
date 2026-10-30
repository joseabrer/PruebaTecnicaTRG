using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace GtMotive.Estimate.Microservice.Api.Respuestas
{
    /// <summary>
    /// Convierte cada <see cref="RespuestasApiAttribute"/> en los metadatos ProducesResponseType que MVC y Swagger
    /// entienden. Se ejecuta una sola vez, al arrancar la aplicación, para cada acción que tiene el atributo.
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

            accion.Filters.Add(new ProducesResponseTypeAttribute(atributo.TipoRespuesta, atributo.CodigoExito));

            if (atributo.SolicitudInvalida)
            {
                AgregarProblema(accion, StatusCodes.Status400BadRequest);
            }

            if (atributo.NoEncontrado)
            {
                AgregarProblema(accion, StatusCodes.Status404NotFound);
            }

            if (atributo.Conflicto)
            {
                AgregarProblema(accion, StatusCodes.Status409Conflict);
            }
        }

        private static void AgregarProblema(ActionModel accion, int codigo)
        {
            accion.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), codigo));
        }
    }
}
