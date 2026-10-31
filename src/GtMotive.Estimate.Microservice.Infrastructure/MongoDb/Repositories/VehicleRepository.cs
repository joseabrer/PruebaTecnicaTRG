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
                // El índice único sobre la matrícula es lo que hace esto seguro con peticiones simultáneas.
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

            // La fecha de fabricación se guarda como texto yyyy-MM-dd, así que comparar el texto equivale a comparar las fechas.
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

            // Compare-and-swap: solo se aplica si el estado guardado es distinto del nuevo, así que de dos
            // peticiones simultáneas que cargaron el mismo vehículo solo una puede cambiarlo.
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
