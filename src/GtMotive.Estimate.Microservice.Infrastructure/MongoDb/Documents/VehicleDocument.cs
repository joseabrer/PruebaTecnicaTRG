using System;
using System.Globalization;
using GtMotive.Estimate.Microservice.Domain.Vehicles;

namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Documents
{
    /// <summary>
    /// Modelo de persistencia de un <see cref="Vehicle"/>. El dominio queda libre de cualquier detalle de almacenamiento:
    /// el agregado se reconstruye mediante <see cref="Vehicle.Restore"/>.
    /// </summary>
    internal sealed class VehicleDocument
    {
        private const string DateFormat = "yyyy-MM-dd";

        public Guid Id { get; set; }

        public string LicensePlate { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public string ManufactureDate { get; set; }

        public VehicleStatus Status { get; set; }

        public static string FormatDate(DateOnly date)
        {
            return date.ToString(DateFormat, CultureInfo.InvariantCulture);
        }

        public static VehicleDocument From(Vehicle vehicle)
        {
            return new VehicleDocument
            {
                Id = vehicle.Id,
                LicensePlate = vehicle.LicensePlate.Value,
                Brand = vehicle.Brand,
                Model = vehicle.Model,
                ManufactureDate = FormatDate(vehicle.ManufactureDate),
                Status = vehicle.Status
            };
        }

        public Vehicle ToDomain()
        {
            return Vehicle.Restore(
                Id,
                Domain.Vehicles.LicensePlate.Create(LicensePlate),
                Brand,
                Model,
                DateOnly.ParseExact(ManufactureDate, DateFormat, CultureInfo.InvariantCulture),
                Status);
        }
    }
}
