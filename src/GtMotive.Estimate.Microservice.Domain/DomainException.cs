using System;
using System.Diagnostics.CodeAnalysis;

[assembly: CLSCompliant(false)]

namespace GtMotive.Estimate.Microservice.Domain
{
    /// <summary>
    /// Excepción de dominio.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DomainException : Exception
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="DomainException"/>.
        /// </summary>
        public DomainException()
            : base()
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="DomainException"/>.
        /// </summary>
        /// <param name="message">Mensaje.</param>
        public DomainException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="DomainException"/>.
        /// </summary>
        /// <param name="message">El mensaje de error que explica el motivo de la excepción.</param>
        /// <param name="innerException">La excepción que es la causa de la excepción actual, o una referencia nula si no se especifica ninguna excepción interna.</param>
        public DomainException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
