using System;
using System.Diagnostics.CodeAnalysis;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GtMotive.Estimate.Microservice.Infrastructure.Logging
{
    /// <summary>
    /// Clase que implementa la interfaz IAppLogger.
    /// </summary>
    /// <typeparam name="T">Tipo de objeto.</typeparam>
    public class LoggerAdapter<T> : IAppLogger<T>
    {
        /// <summary>
        /// Interfaz del logger.
        /// </summary>
        private readonly ILogger<T> _logger;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="LoggerAdapter{T}"/>.
        /// </summary>
        /// <param name="loggerFactory">Tipo que se usa para configurar el sistema de logging y crear instancias de ILogger.</param>
        public LoggerAdapter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<T>();
        }

        /// <summary>
        /// Da formato y escribe un mensaje de log de advertencia.
        /// </summary>
        /// <param name="message">El mensaje.</param>
        /// <param name="args">Array que contiene cero o más objetos que se van a formatear.</param>
        [ExcludeFromCodeCoverage]
        public void LogWarning(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
        }

        /// <summary>
        /// Da formato y escribe un mensaje de log informativo.
        /// </summary>
        /// <param name="message">Un mensaje.</param>
        /// <param name="args">Array que contiene cero o más objetos que se van a formatear.</param>
        public void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }

        /// <summary>
        /// Da formato y escribe un mensaje de log de error.
        /// </summary>
        /// <param name="exception">Una excepción.</param>
        /// <param name="message">Un mensaje.</param>
        /// <param name="args">Array que contiene cero o más objetos que se van a formatear.</param>
        [ExcludeFromCodeCoverage]
        public void LogError(Exception exception, string message, params object[] args)
        {
            _logger.LogError(exception, message, args);
        }

        /// <summary>
        /// Da formato y escribe un mensaje de log de depuración.
        /// </summary>
        /// <param name="message">Un mensaje.</param>
        /// <param name="args">Array que contiene cero o más objetos que se van a formatear.</param>
        public void LogDebug(string message, params object[] args)
        {
            _logger.LogDebug(message, args);
        }
    }
}
