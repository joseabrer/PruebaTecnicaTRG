using System;
using GtMotive.Estimate.Microservice.Api.Models;
using GtMotive.Estimate.Microservice.Api.Respuestas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GtMotive.Estimate.Microservice.Api.Filters
{
    /// <summary>
    /// Mete en el sobre <see cref="RespuestaApi{T}"/> los errores que genera el propio MVC y devuelve como
    /// ProblemDetails, por ejemplo un 415 cuando el cuerpo no se envía como application/json. Así un cliente lee
    /// siempre los mismos cuatro campos, vengan del código de la API o del framework.
    /// </summary>
    public sealed class EnvolverProblemasFilter : IAlwaysRunResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Result is not ObjectResult { Value: ProblemDetails problema } resultado)
            {
                return;
            }

            var mensaje = problema.Title ?? Mensajes.TituloErrorInterno;
            var respuesta = new RespuestaApi<object>(
                mensaje,
                [problema.Detail ?? mensaje]);

            // The body is no longer a problem document, so it goes out as plain JSON.
            resultado.ContentTypes.Clear();
            resultado.Value = respuesta;
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            // Nothing to do once the result has been written.
        }
    }
}
