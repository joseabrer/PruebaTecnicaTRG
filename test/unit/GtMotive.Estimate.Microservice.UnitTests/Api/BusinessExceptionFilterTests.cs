using System;
using System.Collections.Generic;
using FluentAssertions;
using GtMotive.Estimate.Microservice.Api.Filters;
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
            var problem = result.Value.Should().BeOfType<ProblemDetails>().Subject;
            problem.Status.Should().Be(StatusCodes.Status500InternalServerError);
            problem.Title.Should().Be(Mensajes.TituloErrorInterno);
            problem.Detail.Should().Be(Mensajes.DetalleErrorInterno);
            problem.Detail.Should().NotContain("mongo").And.NotContain("refused");
        }

        [Fact]
        public void OnExceptionKeepsTheBusinessMessageOfADomainException()
        {
            var context = NewContext(new DomainException("El vehículo es demasiado antiguo."));

            _filter.OnException(context);

            var result = context.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            result.Value.Should().BeOfType<ProblemDetails>().Which.Detail.Should().Be("El vehículo es demasiado antiguo.");
        }

        private static ExceptionContext NewContext(Exception exception)
        {
            var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
            var filters = new List<IFilterMetadata>();

            return new ExceptionContext(actionContext, filters) { Exception = exception };
        }
    }
}
