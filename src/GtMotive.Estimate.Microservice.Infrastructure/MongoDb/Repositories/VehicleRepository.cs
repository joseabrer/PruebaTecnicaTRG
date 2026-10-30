using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Documents;
using MongoDB.Driver;

namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Repositories
{
    internal sealed class VehicleRepository(MongoService mongo) : IVehicleRepository
    {
        private readonly MongoService _mongo = mongo;

        public async Task Add(Vehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            var collection = await _mongo.GetVehicles();

            try
            {
                await collection.InsertOneAsync(VehicleDocument.From(vehicle));
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                // The unique index on the license plate is what makes this safe under concurrency.
                throw new LicensePlateAlreadyRegisteredException($"Ya existe un vehículo con la matrícula {vehicle.LicensePlate.Value}.");
            }
        }

        public async Task<Vehicle> GetById(Guid id)
        {
            var collection = await _mongo.GetVehicles();
            var document = await collection.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (document is null)
            {
                throw new VehicleNotFoundException($"El vehículo {id} no existe.");
            }

            return document.ToDomain();
        }

        public async Task<IReadOnlyCollection<Vehicle>> ListAvailable(DateOnly oldestManufactureDate)
        {
            var collection = await _mongo.GetVehicles();

            // The manufacture date is stored as yyyy-MM-dd text, so comparing the text compares the dates.
            var filter = Builders<VehicleDocument>.Filter.And(
                Builders<VehicleDocument>.Filter.Eq(x => x.Status, VehicleStatus.Available),
                Builders<VehicleDocument>.Filter.Gte(x => x.ManufactureDate, VehicleDocument.FormatDate(oldestManufactureDate)));

            var documents = await collection
                .Find(filter)
                .SortBy(x => x.LicensePlate)
                .ToListAsync();

            return documents.ConvertAll(x => x.ToDomain());
        }

        public async Task Update(Vehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            var collection = await _mongo.GetVehicles();

            // Compare-and-swap: only applies if the stored status is different from the new one, so of two
            // concurrent requests that loaded the same vehicle only one can change it.
            var result = await collection.UpdateOneAsync(
                x => x.Id == vehicle.Id && x.Status != vehicle.Status,
                Builders<VehicleDocument>.Update.Set(x => x.Status, vehicle.Status));

            if (result.MatchedCount == 0)
            {
                throw new ConcurrentModificationException($"El vehículo {vehicle.Id} ha sido modificado por otra petición.");
            }
        }
    }
}
