using System;

namespace GtMotive.Estimate.Microservice.Domain.Interfaces
{
    /// <summary>
    /// Este tipo elimina la necesidad de depender directamente de los tipos de logging de ASP.NET Core.
    /// </summary>
    /// <typeparam name="T">El tipo cuyo nombre se usa como nombre de categoría del logger.</typeparam>
    public interface IAppLogger<T>
    {
        /// <summary>
        /// Da formato y escribe un mensaje de log informativo.
        /// </summary>
        /// <param name="message">Cadena de formato del mensaje de log en formato de plantilla de mensaje. Ejemplo: "El usuario {User}
        /// inició sesión desde {Address}".</param>
        /// <param name="args">Un array de objetos que contiene cero o más objetos que se van a formatear.</param>
        void LogInformation(string message, params object[] args);

        /// <summary>
        /// Da formato y escribe un mensaje de log de advertencia.
        /// </summary>
        /// <param name="message">Cadena de formato del mensaje de log en formato de plantilla de mensaje. Ejemplo: "El usuario {User}
        /// inició sesión desde {Address}".</param>
        /// <param name="args">Un array de objetos que contiene cero o más objetos que se van a formatear.</param>
        void LogWarning(string message, params object[] args);

        /// <summary>
        /// Da formato y escribe un mensaje de log de error.
        /// </summary>
        /// <param name="exception">Excepción capturada.</param>
        /// <param name="message">Cadena de formato del mensaje de log en formato de plantilla de mensaje. Ejemplo: "El usuario {User}
        /// inició sesión desde {Address}".</param>
        /// <param name="args">Un array de objetos que contiene cero o más objetos que se van a formatear.</param>
        void LogError(Exception exception, string message, params object[] args);

        /// <summary>
        /// Da formato y escribe un mensaje de log de depuración.
        /// </summary>
        /// <param name="message">Cadena de formato del mensaje de log en formato de plantilla de mensaje. Ejemplo: "El usuario {User}
        /// inició sesión desde {Address}".</param>
        /// <param name="args">Un array de objetos que contiene cero o más objetos que se van a formatear.</param>
        void LogDebug(string message, params object[] args);
    }
}
