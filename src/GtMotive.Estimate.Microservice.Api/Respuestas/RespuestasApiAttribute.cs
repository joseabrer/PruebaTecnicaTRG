using System;
using Microsoft.AspNetCore.Http;

namespace GtMotive.Estimate.Microservice.Api.Respuestas
{
    /// <summary>
    /// Declara, en un solo sitio, las respuestas HTTP de una acción de un controller. Sustituye a la lista de
    /// atributos ProducesResponseType: la respuesta de éxito más las respuestas de error estándar
    /// (400, 404 y 409, todas ellas dentro del sobre RespuestaApi) que se aplican a la acción. Se puede usar en cualquier controller.
    /// </summary>
    /// <param name="tipoRespuesta">Tipo de los datos de la respuesta de éxito, que viajan dentro del sobre RespuestaApi.</param>
    /// <param name="codigoExito">Código de estado de la respuesta de éxito. Es 200 salvo que se indique otro.</param>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class RespuestasApiAttribute(Type tipoRespuesta, int codigoExito = StatusCodes.Status200OK) : Attribute
    {
        /// <summary>
        /// Obtiene el tipo de los datos de la respuesta de éxito.
        /// </summary>
        public Type TipoRespuesta { get; } = tipoRespuesta;

        /// <summary>
        /// Obtiene el código de estado de la respuesta de éxito.
        /// </summary>
        public int CodigoExito { get; } = codigoExito;

        /// <summary>
        /// Obtiene o establece un valor que indica si la acción puede responder 400. Es true salvo que se desactive.
        /// </summary>
        public bool SolicitudInvalida { get; set; } = true;

        /// <summary>
        /// Obtiene o establece un valor que indica si la acción puede responder 404.
        /// </summary>
        public bool NoEncontrado { get; set; }

        /// <summary>
        /// Obtiene o establece un valor que indica si la acción puede responder 409.
        /// </summary>
        public bool Conflicto { get; set; }
    }
}
