using System;
using GtMotive.Estimate.Microservice.Api.Respuestas;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GtMotive.Estimate.Microservice.Api.Filters
{
    public sealed class BusinessExceptionFilter(IAppLogger<BusinessExceptionFilter> appLogger) : IExceptionFilter
    {
        private readonly IAppLogger<BusinessExceptionFilter> _appLogger = appLogger;

        public void OnException(ExceptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            _appLogger.LogError(context.Exception, "Excepción capturada en BusinessExceptionFilter.");

            if (context.Exception is DomainException)
            {
                var problemDetails = new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Status = StatusCodes.Status400BadRequest,
                    Title = Mensajes.TituloSolicitudInvalida,
                    Detail = context.Exception.Message,
                    Instance = context.HttpContext.Request.Path,
                };

                _appLogger.LogWarning("Excepción de dominio: {status} - {detail}", problemDetails.Status, problemDetails.Detail);

                context.Result = new BadRequestObjectResult(problemDetails);
                context.Exception = null;
            }
            else
            {
                // The message of an unexpected exception can reveal internal details (hosts, ports, driver internals),
                // so it is only logged, never returned to the client.
                var problemDetails = new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                    Status = StatusCodes.Status500InternalServerError,
                    Title = Mensajes.TituloErrorInterno,
                    Detail = Mensajes.DetalleErrorInterno,
                    Instance = context.HttpContext.Request.Path,
                };

                _appLogger.LogError(context.Exception, "Excepción no controlada");

                context.Result = new InternalServerErrorObjectResult(problemDetails);
                context.Exception = null;
            }
        }
    }
}
