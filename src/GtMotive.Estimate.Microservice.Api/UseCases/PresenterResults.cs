using GtMotive.Estimate.Microservice.Api.Models;
using GtMotive.Estimate.Microservice.Api.Respuestas;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases
{
    /// <summary>
    /// Respuestas de error compartidas por los presenters, dentro del mismo sobre <see cref="RespuestaApi{T}"/> que usan
    /// el resto de respuestas de la API.
    /// </summary>
    internal static class PresenterResults
    {
        public static IActionResult NotFound(string message)
        {
            var respuesta = new RespuestaApi<object>(
                Mensajes.TituloNoEncontrado,
                [message]);

            return new NotFoundObjectResult(respuesta);
        }

        public static IActionResult Conflict(string message)
        {
            var respuesta = new RespuestaApi<object>(
                Mensajes.TituloConflicto,
                [message]);

            return new ConflictObjectResult(respuesta);
        }
    }
}
