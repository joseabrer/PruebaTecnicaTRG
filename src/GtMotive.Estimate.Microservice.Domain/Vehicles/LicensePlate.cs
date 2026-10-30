namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Matrícula de un vehículo. Objeto de valor: inmutable y comparado por valor.
    /// </summary>
    public sealed record LicensePlate
    {
        private const int MaxLength = 15;

        private LicensePlate(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Obtiene el texto normalizado de la matrícula (sin espacios en los extremos y en mayúsculas).
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Crea una matrícula a partir de texto sin procesar.
        /// </summary>
        /// <param name="text">Texto sin procesar escrito por el usuario.</param>
        /// <returns>La matrícula normalizada.</returns>
        public static LicensePlate Create(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new DomainException("La matrícula es obligatoria.");
            }

            var normalized = text.Trim().ToUpperInvariant();

            if (normalized.Length > MaxLength)
            {
                throw new DomainException("La matrícula es demasiado larga.");
            }

            return new LicensePlate(normalized);
        }
    }
}
