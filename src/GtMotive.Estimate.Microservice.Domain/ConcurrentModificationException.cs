namespace GtMotive.Estimate.Microservice.Domain
{
    /// <summary>
    /// Otra petición modificó el agregado desde que se cargó, así que la actualización no se aplicó.
    /// </summary>
    public sealed class ConcurrentModificationException : DomainException
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ConcurrentModificationException"/>.
        /// </summary>
        /// <param name="message">Mensaje.</param>
        public ConcurrentModificationException(string message)
            : base(message)
        {
        }
    }
}
