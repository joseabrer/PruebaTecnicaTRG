using GtMotive.Estimate.Microservice.Api.Respuestas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases
{
    /// <summary>
    /// Respuestas de error compartidas por los presenters, con la misma forma de ProblemDetails que usa el filtro de excepciones.
    /// </summary>
    internal static class PresenterResults
    {
        public static IActionResult NotFound(string message)
        {
            var problemDetails = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Status = StatusCodes.Status404NotFound,
                Title = Mensajes.TituloNoEncontrado,
                Detail = message
            };

            return new NotFoundObjectResult(problemDetails);
        }

        public static IActionResult Conflict(string message)
        {
            var problemDetails = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                Status = StatusCodes.Status409Conflict,
                Title = Mensajes.TituloConflicto,
                Detail = message
            };

            return new ConflictObjectResult(problemDetails);
        }
    }
}
