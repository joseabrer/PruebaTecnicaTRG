using System;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Infrastructure.Interfaces;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb;
using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace GtMotive.Estimate.Microservice.Infrastructure
{
    public static class MongoDbExtensions
    {
        /// <summary>
        /// Registra los adaptadores de MongoDB de los puertos de persistencia. Quien llama debe configurar los ajustes <c>MongoDb</c>.
        /// </summary>
        /// <param name="builder">Constructor de la infraestructura.</param>
        /// <returns>El mismo constructor, para seguir encadenando llamadas.</returns>
        public static IInfrastructureBuilder AddMongoDb(this IInfrastructureBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.Services.AddSingleton<MongoService>();
            builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
            builder.Services.AddScoped<IRentalRepository, RentalRepository>();

            return builder;
        }
    }
}
