using System;
using System.Globalization;

namespace GtMotive.Estimate.Microservice.Domain.Vehicles
{
    /// <summary>
    /// Vehículo de la flota de alquiler. Raíz de agregado: mantiene coherente su propia disponibilidad.
    /// </summary>
    public sealed class Vehicle
    {
        /// <summary>
        /// Antigüedad máxima, en años, de un vehículo que puede formar parte de la flota.
        /// </summary>
        public const int MaxAgeInYears = 5;

        private Vehicle(
            Guid id,
            LicensePlate licensePlate,
            string brand,
            string model,
            DateOnly manufactureDate,
            VehicleStatus status)
        {
            Id = id;
            LicensePlate = licensePlate;
            Brand = brand;
            Model = model;
            ManufactureDate = manufactureDate;
            Status = status;
        }

        /// <summary>
        /// Obtiene el identificador único del vehículo.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Obtiene la matrícula.
        /// </summary>
        public LicensePlate LicensePlate { get; }

        /// <summary>
        /// Obtiene la marca.
        /// </summary>
        public string Brand { get; }

        /// <summary>
        /// Obtiene el modelo.
        /// </summary>
        public string Model { get; }

        /// <summary>
        /// Obtiene la fecha de fabricación.
        /// </summary>
        public DateOnly ManufactureDate { get; }

        /// <summary>
        /// Obtiene la disponibilidad actual.
        /// </summary>
        public VehicleStatus Status { get; private set; }

        /// <summary>
        /// Calcula la fecha de fabricación más antigua que admite la flota en la fecha indicada.
        /// </summary>
        /// <param name="today">Fecha actual, proporcionada por quien llama para que la regla siga siendo determinista.</param>
        /// <returns>La fecha de fabricación más antigua admitida: hoy menos la antigüedad máxima.</returns>
        public static DateOnly OldestAllowedManufactureDate(DateOnly today)
        {
            return today.AddYears(-MaxAgeInYears);
        }

        /// <summary>
        /// Registra un vehículo nuevo en la flota. Es el único sitio donde se aplican las reglas de admisión de la flota.
        /// </summary>
        /// <param name="licensePlate">Matrícula.</param>
        /// <param name="brand">Marca.</param>
        /// <param name="model">Modelo.</param>
        /// <param name="manufactureDate">Fecha de fabricación.</param>
        /// <param name="today">Fecha actual, proporcionada por quien llama para que la regla siga siendo determinista.</param>
        /// <returns>El vehículo nuevo y disponible.</returns>
        public static Vehicle Register(
            LicensePlate licensePlate,
            string brand,
            string model,
            DateOnly manufactureDate,
            DateOnly today)
        {
            ArgumentNullException.ThrowIfNull(licensePlate);

            if (string.IsNullOrWhiteSpace(brand))
            {
                throw new DomainException("La marca es obligatoria.");
            }

            if (string.IsNullOrWhiteSpace(model))
            {
                throw new DomainException("El modelo es obligatorio.");
            }

            if (manufactureDate > today)
            {
                throw new ManufactureDateInFutureException(
                    string.Create(CultureInfo.InvariantCulture, $"La fecha de fabricación {manufactureDate:dd/MM/yyyy} es futura."));
            }

            if (manufactureDate < OldestAllowedManufactureDate(today))
            {
                throw new VehicleTooOldException(
                    string.Create(CultureInfo.InvariantCulture, $"El vehículo se fabricó el {manufactureDate:dd/MM/yyyy}. La flota solo admite vehículos de hasta {MaxAgeInYears} años de antigüedad."));
            }

            return new Vehicle(Guid.NewGuid(), licensePlate, brand.Trim(), model.Trim(), manufactureDate, VehicleStatus.Available);
        }

        /// <summary>
        /// Reconstruye un vehículo a partir de su estado guardado. Las reglas de admisión no se vuelven a aplicar:
        /// que haya envejecido más allá de la antigüedad máxima se decide al listarlo y al alquilarlo, con la fecha de hoy.
        /// </summary>
        /// <param name="id">Identificador.</param>
        /// <param name="licensePlate">Matrícula.</param>
        /// <param name="brand">Marca.</param>
        /// <param name="model">Modelo.</param>
        /// <param name="manufactureDate">Fecha de fabricación.</param>
        /// <param name="status">Disponibilidad guardada.</param>
        /// <returns>El vehículo reconstruido.</returns>
        public static Vehicle Restore(
            Guid id,
            LicensePlate licensePlate,
            string brand,
            string model,
            DateOnly manufactureDate,
            VehicleStatus status)
        {
            ArgumentNullException.ThrowIfNull(licensePlate);

            return new Vehicle(id, licensePlate, brand, model, manufactureDate, status);
        }

        /// <summary>
        /// Indica si el vehículo sigue dentro de la antigüedad máxima que admite la flota en la fecha indicada.
        /// </summary>
        /// <param name="today">Fecha actual, proporcionada por quien llama para que la regla siga siendo determinista.</param>
        /// <returns>True si el vehículo no supera la antigüedad máxima.</returns>
        public bool IsWithinMaximumAge(DateOnly today)
        {
            return ManufactureDate >= OldestAllowedManufactureDate(today);
        }

        /// <summary>
        /// Marca el vehículo como alquilado. Un vehículo que ha superado la antigüedad máxima ya no se puede alquilar.
        /// </summary>
        /// <param name="today">Fecha actual, proporcionada por quien llama para que la regla siga siendo determinista.</param>
        public void Rent(DateOnly today)
        {
            if (Status != VehicleStatus.Available)
            {
                throw new VehicleNotAvailableException($"El vehículo {LicensePlate.Value} no está disponible.");
            }

            if (!IsWithinMaximumAge(today))
            {
                throw new VehicleNotAvailableException(
                    string.Create(CultureInfo.InvariantCulture, $"El vehículo {LicensePlate.Value} se fabricó el {ManufactureDate:dd/MM/yyyy} y supera los {MaxAgeInYears} años de antigüedad máxima de la flota: ya no se puede alquilar."));
            }

            Status = VehicleStatus.Rented;
        }

        /// <summary>
        /// Marca el vehículo como disponible de nuevo.
        /// </summary>
        public void Return()
        {
            if (Status != VehicleStatus.Rented)
            {
                throw new VehicleNotRentedException($"El vehículo {LicensePlate.Value} no está alquilado.");
            }

            Status = VehicleStatus.Available;
        }
    }
}
