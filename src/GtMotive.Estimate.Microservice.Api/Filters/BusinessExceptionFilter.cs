using System;
using GtMotive.Estimate.Microservice.Api.Models;
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
                var respuesta = new RespuestaApi<object>(
                    Mensajes.TituloSolicitudInvalida,
                    [context.Exception.Message]);

                _appLogger.LogWarning("Excepción de dominio: {status} - {detail}", StatusCodes.Status400BadRequest, context.Exception.Message);

                context.Result = new BadRequestObjectResult(respuesta);
                context.Exception = null;
            }
            else
            {
                // The message of an unexpected exception can reveal internal details (hosts, ports, driver internals),
                // so it is only logged, never returned to the client.
                var respuesta = new RespuestaApi<object>(
                    Mensajes.TituloErrorInterno,
                    [Mensajes.DetalleErrorInterno]);

                _appLogger.LogError(context.Exception, "Excepción no controlada");

                context.Result = new InternalServerErrorObjectResult(respuesta);
                context.Exception = null;
            }
        }
    }
}
