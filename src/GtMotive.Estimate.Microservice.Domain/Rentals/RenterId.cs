namespace GtMotive.Estimate.Microservice.Domain.Rentals
{
    /// <summary>
    /// Identificador de la persona que alquila un vehículo (por ejemplo un número de DNI o un email).
    /// Objeto de valor: inmutable y comparado por valor. Se normaliza para que una misma persona no pueda
    /// saltarse la regla de un solo alquiler activo con solo cambiar entre mayúsculas y minúsculas.
    /// </summary>
    public sealed record RenterId
    {
        private const int MaxLength = 100;

        private RenterId(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Obtiene el texto normalizado del identificador (sin espacios en los extremos y en mayúsculas).
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Crea el identificador de la persona que alquila a partir de texto sin procesar.
        /// </summary>
        /// <param name="text">Texto sin procesar escrito por el usuario.</param>
        /// <returns>El identificador normalizado de la persona que alquila.</returns>
        public static RenterId Create(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new DomainException("El identificador del cliente es obligatorio.");
            }

            var normalized = text.Trim().ToUpperInvariant();

            if (normalized.Length > MaxLength)
            {
                throw new DomainException("El identificador del cliente es demasiado largo.");
            }

            return new RenterId(normalized);
        }
    }
}
