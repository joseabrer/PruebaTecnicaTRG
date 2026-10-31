using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GtMotive.Estimate.Microservice.Api.Models
{
    /// <summary>
    /// El sobre en el que viaja toda respuesta de la API, de éxito o de error, para que los clientes lean siempre
    /// los mismos cuatro campos: si fue bien, un mensaje, los datos y la lista de errores.
    /// </summary>
    /// <typeparam name="T">Tipo de los datos que acompañan a una respuesta de éxito.</typeparam>
    public sealed class RespuestaApi<T>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="RespuestaApi{T}"/>.
        /// </summary>
        /// <param name="datos">Datos de la respuesta de éxito.</param>
        /// <param name="mensaje">Mensaje que acompaña a los datos.</param>
        public RespuestaApi(T datos, string mensaje)
        {
            Exito = true;
            Mensaje = mensaje;
            Datos = datos;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="RespuestaApi{T}"/>.
        /// </summary>
        /// <param name="mensaje">Mensaje que resume el error.</param>
        /// <param name="errores">Detalle de cada error.</param>
        public RespuestaApi(string mensaje, IReadOnlyList<string> errores)
        {
            Exito = false;
            Mensaje = mensaje;
            Errores = errores;
        }

        /// <summary>
        /// Obtiene un valor que indica si la petición se resolvió bien.
        /// </summary>
        [Required]
        public bool Exito { get; }

        /// <summary>
        /// Obtiene el mensaje de la respuesta: lo que se hizo, o el motivo del error.
        /// </summary>
        [Required]
        public string Mensaje { get; }

        /// <summary>
        /// Obtiene los datos de la respuesta, o null si es un error.
        /// </summary>
        public T Datos { get; }

        /// <summary>
        /// Obtiene el detalle de los errores, o null si la petición se resolvió bien.
        /// </summary>
        public IReadOnlyList<string> Errores { get; }
    }
}
