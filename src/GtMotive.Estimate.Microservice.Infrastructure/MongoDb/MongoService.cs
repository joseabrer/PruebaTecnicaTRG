using System;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Documents;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb
{
    public class MongoService
    {
        private const string VehiclesCollectionName = "vehicles";
        private const string RentalsCollectionName = "rentals";

        private static readonly Lock BsonLock = new();

        // Version 3 of the driver leaves the Guid representation unspecified, so it has to be set explicitly.
        private static readonly GuidSerializer StandardGuid = new(GuidRepresentation.Standard);
        private static bool _bsonClassesRegistered;

        private readonly Lock _indexesLock = new();
        private readonly IMongoDatabase _database;
        private Task _indexes;

        public MongoService(IOptions<MongoDbSettings> options)
        {
            ArgumentNullException.ThrowIfNull(options);

            var settings = options.Value;

            if (string.IsNullOrWhiteSpace(settings.ConnectionString))
            {
                throw new InvalidOperationException("Falta la configuración MongoDb:ConnectionString.");
            }

            if (string.IsNullOrWhiteSpace(settings.MongoDbDatabaseName))
            {
                throw new InvalidOperationException("Falta la configuración MongoDb:MongoDbDatabaseName.");
            }

            RegisterBsonClasses();

            MongoClient = new MongoClient(settings.ConnectionString);
            _database = MongoClient.GetDatabase(settings.MongoDbDatabaseName);
        }

        public MongoClient MongoClient { get; }

        internal async Task<IMongoCollection<VehicleDocument>> GetVehicles()
        {
            await EnsureIndexes();
            return _database.GetCollection<VehicleDocument>(VehiclesCollectionName);
        }

        internal async Task<IMongoCollection<RentalDocument>> GetRentals()
        {
            await EnsureIndexes();
            return _database.GetCollection<RentalDocument>(RentalsCollectionName);
        }

        private static void RegisterBsonClasses()
        {
            lock (BsonLock)
            {
                if (_bsonClassesRegistered)
                {
                    return;
                }

                var conventions = new ConventionPack
                {
                    new CamelCaseElementNameConvention(),
                    new EnumRepresentationConvention(BsonType.String),
                    new IgnoreExtraElementsConvention(true)
                };

                ConventionRegistry.Register(
                    "RentalDocuments",
                    conventions,
                    type => type == typeof(VehicleDocument) || type == typeof(RentalDocument));

                BsonClassMap.RegisterClassMap<VehicleDocument>(map =>
                {
                    map.AutoMap();
                    map.IdMemberMap.SetSerializer(StandardGuid);
                });
                BsonClassMap.RegisterClassMap<RentalDocument>(map =>
                {
                    map.AutoMap();
                    map.IdMemberMap.SetSerializer(StandardGuid);
                    map.GetMemberMap(x => x.VehicleId).SetSerializer(StandardGuid);
                });

                _bsonClassesRegistered = true;
            }
        }

        // The indexes are what makes the rules safe under concurrency, so they are created on first use
        // (idempotent) instead of in a hosted service: that way they also exist when there is no host,
        // for instance in the functional tests. A failed attempt is retried on the next call.
        private Task EnsureIndexes()
        {
            lock (_indexesLock)
            {
                if (_indexes is null || _indexes.IsFaulted || _indexes.IsCanceled)
                {
                    _indexes = CreateIndexes();
                }

                return _indexes;
            }
        }

        private async Task CreateIndexes()
        {
            var vehicles = _database.GetCollection<VehicleDocument>(VehiclesCollectionName);

            await vehicles.Indexes.CreateManyAsync(
            [
                new CreateIndexModel<VehicleDocument>(
                    Builders<VehicleDocument>.IndexKeys.Ascending(x => x.LicensePlate),
                    new CreateIndexOptions { Name = "ux_vehicles_license_plate", Unique = true }),
                new CreateIndexModel<VehicleDocument>(
                    Builders<VehicleDocument>.IndexKeys.Ascending(x => x.Status),
                    new CreateIndexOptions { Name = "ix_vehicles_status" })
            ]);

            var rentals = _database.GetCollection<RentalDocument>(RentalsCollectionName);

            // A person can have only one active rental: unique per renter, but only among the active ones.
            await rentals.Indexes.CreateOneAsync(
                new CreateIndexModel<RentalDocument>(
                    Builders<RentalDocument>.IndexKeys.Ascending(x => x.RenterId),
                    new CreateIndexOptions<RentalDocument>
                    {
                        Name = "ux_rentals_active_renter",
                        Unique = true,
                        PartialFilterExpression = Builders<RentalDocument>.Filter.Eq(x => x.Active, true)
                    }));
        }
    }
}
