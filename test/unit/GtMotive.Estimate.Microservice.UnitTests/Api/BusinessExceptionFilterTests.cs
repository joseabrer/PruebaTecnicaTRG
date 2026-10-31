using System;
using System.Collections.Generic;
using FluentAssertions;
using GtMotive.Estimate.Microservice.Api.Filters;
using GtMotive.Estimate.Microservice.Api.Models;
using GtMotive.Estimate.Microservice.Api.Respuestas;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace GtMotive.Estimate.Microservice.UnitTests.Api
{
    /// <summary>
    /// Tests unitarios del filtro de excepciones. Son los tests de regresión de un hallazgo de seguridad:
    /// el mensaje de una excepción inesperada puede revelar detalles internos, así que no debe llegar al cliente.
    /// </summary>
    public sealed class BusinessExceptionFilterTests
    {
        private readonly BusinessExceptionFilter _filter = new(new Mock<IAppLogger<BusinessExceptionFilter>>().Object);

        [Fact]
        public void OnExceptionHidesTheInternalMessageOfAnUnexpectedError()
        {
            var context = NewContext(new InvalidOperationException("the server mongo:27017 refused the connection"));

            _filter.OnException(context);

            var result = context.Result.Should().BeOfType<InternalServerErrorObjectResult>().Subject;
            result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            var respuesta = result.Value.Should().BeOfType<RespuestaApi<object>>().Subject;
            respuesta.Exito.Should().BeFalse();
            respuesta.Mensaje.Should().Be(Mensajes.TituloErrorInterno);
            var detalle = respuesta.Errores.Should().ContainSingle().Subject;
            detalle.Should().Be(Mensajes.DetalleErrorInterno);
            detalle.Should().NotContain("mongo").And.NotContain("refused");
        }

        [Fact]
        public void OnExceptionKeepsTheBusinessMessageOfADomainException()
        {
            var context = NewContext(new DomainException("El vehículo es demasiado antiguo."));

            _filter.OnException(context);

            var result = context.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            var respuesta = result.Value.Should().BeOfType<RespuestaApi<object>>().Subject;
            respuesta.Exito.Should().BeFalse();
            respuesta.Mensaje.Should().Be(Mensajes.TituloSolicitudInvalida);
            respuesta.Errores.Should().ContainSingle().Which.Should().Be("El vehículo es demasiado antiguo.");
        }

        private static ExceptionContext NewContext(Exception exception)
        {
            var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
            var filters = new List<IFilterMetadata>();

            return new ExceptionContext(actionContext, filters) { Exception = exception };
        }
    }
}
