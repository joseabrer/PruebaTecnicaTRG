using System.Collections.Generic;

namespace GtMotive.Estimate.Microservice.Domain.Interfaces
{
    /// <summary>
    /// Envía eventos, métricas y otra telemetría.
    /// </summary>
    public interface ITelemetry
    {
        /// <summary>
        /// Acciones de los usuarios y otros eventos. Se usa para seguir el comportamiento de los usuarios o para vigilar el rendimiento.
        /// Inserta llamadas a TrackEvent en el código para contar distintos eventos. Con qué frecuencia los usuarios eligen una función concreta,
        /// con qué frecuencia alcanzan objetivos concretos o quizá con qué frecuencia cometen determinados tipos de errores.
        /// </summary>
        /// <param name="eventName">Un nombre para el evento. Longitud máxima 150.</param>
        /// <param name="properties">Valores de texto con nombre que se pueden usar para buscar y filtrar eventos.</param>
        /// <param name="metrics">Mediciones numéricas asociadas a este evento.</param>
        void TrackEvent(
            string eventName,
            IDictionary<string, string> properties = null,
            IDictionary<string, double> metrics = null);

        /// <summary>
        /// Mediciones de rendimiento, como la longitud de las colas, que no están relacionadas con eventos concretos.
        /// Sirve para obtener métricas de gráficos que no están ligadas a eventos concretos. Por ejemplo, se podría vigilar la longitud de una cola
        /// a intervalos regulares. Con las métricas, las mediciones individuales interesan menos que las variaciones y las tendencias,
        /// y por eso resultan útiles los gráficos estadísticos.
        /// </summary>
        /// <param name="name">El nombre de la métrica. Longitud máxima 150.</param>
        /// <param name="value">El valor de la métrica. Es la suma si se basa en más de una muestra.</param>
        /// <param name="properties">Valores de texto con nombre que se pueden usar para buscar y clasificar mensajes de traza.</param>
        void TrackMetric(
            string name,
            double value,
            IDictionary<string, string> properties = null);
    }
}
