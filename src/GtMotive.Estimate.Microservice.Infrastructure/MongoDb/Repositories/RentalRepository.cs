using System;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Rentals;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Documents;
using MongoDB.Driver;

namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Repositories
{
    internal sealed class RentalRepository(MongoService mongo) : IRentalRepository
    {
        private readonly MongoService _mongo = mongo;

        public async Task Add(Rental rental)
        {
            ArgumentNullException.ThrowIfNull(rental);

            var collection = await _mongo.GetRentals();

            try
            {
                await collection.InsertOneAsync(RentalDocument.From(rental));
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                // The partial unique index on the renter (active rentals only) is what makes this safe under concurrency.
                throw new RenterAlreadyHasActiveRentalException($"El cliente {rental.RenterId.Value} ya tiene un vehículo alquilado.");
            }
        }

        public async Task<Rental> GetById(Guid id)
        {
            var collection = await _mongo.GetRentals();
            var document = await collection.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (document is null)
            {
                throw new RentalNotFoundException($"El alquiler {id} no existe.");
            }

            return document.ToDomain();
        }

        public async Task<bool> ExistsActiveByRenter(RenterId renterId)
        {
            ArgumentNullException.ThrowIfNull(renterId);

            var collection = await _mongo.GetRentals();
            var renter = renterId.Value;

            return await collection.Find(x => x.RenterId == renter && x.Active).AnyAsync();
        }

        public async Task Update(Rental rental)
        {
            ArgumentNullException.ThrowIfNull(rental);

            var collection = await _mongo.GetRentals();

            // Compare-and-swap: only applies while the stored rental is still active, so a rental cannot
            // be closed twice by two concurrent requests.
            var result = await collection.UpdateOneAsync(
                x => x.Id == rental.Id && x.Active,
                Builders<RentalDocument>.Update
                    .Set(x => x.ReturnedAt, rental.ReturnedAt?.UtcDateTime)
                    .Set(x => x.Active, rental.IsActive));

            if (result.MatchedCount == 0)
            {
                throw new ConcurrentModificationException($"El alquiler {rental.Id} ha sido modificado por otra petición.");
            }
        }
    }
}
