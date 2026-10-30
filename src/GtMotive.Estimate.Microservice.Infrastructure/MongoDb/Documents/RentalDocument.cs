using System;
using GtMotive.Estimate.Microservice.Domain.Rentals;

namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Documents
{
    /// <summary>
    /// Modelo de persistencia de un <see cref="Rental"/>. <see cref="Active"/> se guarda de forma explícita porque el
    /// índice único parcial que impone "un alquiler activo por persona" está definido sobre él.
    /// </summary>
    internal sealed class RentalDocument
    {
        public Guid Id { get; set; }

        public Guid VehicleId { get; set; }

        public string RenterId { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime? ReturnedAt { get; set; }

        public bool Active { get; set; }

        public static RentalDocument From(Rental rental)
        {
            return new RentalDocument
            {
                Id = rental.Id,
                VehicleId = rental.VehicleId,
                RenterId = rental.RenterId.Value,
                StartedAt = rental.StartedAt.UtcDateTime,
                ReturnedAt = rental.ReturnedAt?.UtcDateTime,
                Active = rental.IsActive
            };
        }

        public Rental ToDomain()
        {
            return Rental.Restore(
                Id,
                VehicleId,
                Domain.Rentals.RenterId.Create(RenterId),
                ToUtcOffset(StartedAt),
                ReturnedAt is null ? null : ToUtcOffset(ReturnedAt.Value));
        }

        private static DateTimeOffset ToUtcOffset(DateTime utc)
        {
            return new DateTimeOffset(DateTime.SpecifyKind(utc, DateTimeKind.Utc));
        }
    }
}
