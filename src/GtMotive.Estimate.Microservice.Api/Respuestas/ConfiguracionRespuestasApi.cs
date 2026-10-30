using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.Respuestas
{
    /// <summary>
    /// Configura cómo responde la API: registra la convención que hay detrás de <see cref="RespuestasApiAttribute"/>
    /// y hace que todos los mensajes que el framework genera por su cuenta (errores de enlace, errores de validación, títulos
    /// de las respuestas de error) salgan en español.
    /// </summary>
    public static class ConfiguracionRespuestasApi
    {
        private static readonly Dictionary<int, string> Titulos = new()
        {
            [StatusCodes.Status400BadRequest] = Mensajes.TituloSolicitudInvalida,
            [StatusCodes.Status401Unauthorized] = "No autenticado",
            [StatusCodes.Status403Forbidden] = "Acceso denegado",
            [StatusCodes.Status404NotFound] = Mensajes.TituloNoEncontrado,
            [StatusCodes.Status406NotAcceptable] = "Formato de respuesta no aceptable",
            [StatusCodes.Status409Conflict] = Mensajes.TituloConflicto,
            [StatusCodes.Status415UnsupportedMediaType] = "Tipo de contenido no admitido",
            [StatusCodes.Status422UnprocessableEntity] = "Entidad no procesable",
            [StatusCodes.Status500InternalServerError] = Mensajes.TituloErrorInterno
        };

        /// <summary>
        /// Registra la convención de <see cref="RespuestasApiAttribute"/> y los mensajes de enlace en español.
        /// </summary>
        /// <param name="opciones">Opciones de MVC.</param>
        public static void ConfigurarMvc(MvcOptions opciones)
        {
            ArgumentNullException.ThrowIfNull(opciones);

            opciones.Conventions.Add(new RespuestasApiConvencion());

            var mensajes = opciones.ModelBindingMessageProvider;
            mensajes.SetMissingRequestBodyRequiredValueAccessor(() => Mensajes.CuerpoObligatorio);
            mensajes.SetMissingKeyOrValueAccessor(() => Mensajes.ValorNoValido);
            mensajes.SetMissingBindRequiredValueAccessor(campo => $"Falta el campo {campo}.");
            mensajes.SetValueMustNotBeNullAccessor(valor => $"El valor '{valor}' es obligatorio.");
            mensajes.SetValueIsInvalidAccessor(valor => $"El valor '{valor}' no es válido.");
            mensajes.SetAttemptedValueIsInvalidAccessor((valor, campo) => $"El valor '{valor}' no es válido para {campo}.");
            mensajes.SetUnknownValueIsInvalidAccessor(campo => $"El valor indicado no es válido para {campo}.");
            mensajes.SetValueMustBeANumberAccessor(campo => $"El campo {campo} debe ser un número.");
        }

        /// <summary>
        /// Pone títulos en español a las respuestas de error y construye la respuesta 400 de una petición inválida.
        /// </summary>
        /// <param name="opciones">Opciones de comportamiento de los controllers de la API.</param>
        public static void ConfigurarComportamiento(ApiBehaviorOptions opciones)
        {
            ArgumentNullException.ThrowIfNull(opciones);

            foreach (var titulo in Titulos)
            {
                if (opciones.ClientErrorMapping.TryGetValue(titulo.Key, out var datos))
                {
                    datos.Title = titulo.Value;
                }
            }

            opciones.InvalidModelStateResponseFactory = CrearRespuestaSolicitudInvalida;
        }

        private static BadRequestObjectResult CrearRespuestaSolicitudInvalida(ActionContext contexto)
        {
            var errores = new Dictionary<string, string[]>();

            foreach (var campo in contexto.ModelState)
            {
                // The error of a JSON that cannot be read is keyed by its JSON path ($.fechaFabricacion) and carries
                // a text the framework generates in English that shows internals (paths, positions): it is replaced.
                var esErrorDeJson = campo.Key.StartsWith('$');

                var mensajes = campo.Value.Errors
                    .Select(error => esErrorDeJson || error.Exception is not null || string.IsNullOrWhiteSpace(error.ErrorMessage) ? Mensajes.ValorNoValido : error.ErrorMessage)
                    .Distinct()
                    .ToArray();

                if (mensajes.Length > 0)
                {
                    errores[campo.Key] = mensajes;
                }
            }

            var problema = new ValidationProblemDetails(errores)
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Title = Mensajes.TituloSolicitudInvalida,
                Status = StatusCodes.Status400BadRequest,
                Detail = Mensajes.DetalleSolicitudInvalida,
                Instance = contexto.HttpContext.Request.Path
            };

            return new BadRequestObjectResult(problema)
            {
                ContentTypes = { "application/problem+json" }
            };
        }
    }
}
