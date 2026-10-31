using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.Domain.Interfaces;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GtMotive.Estimate.Microservice.Host.DevelopmentData
{
    /// <summary>
    /// Carga una pequeña flota de demostración cuando la aplicación arranca en Development, para que la API tenga vehículos que listar y alquilar
    /// desde la primera petición. Pasa por el dominio (<see cref="Vehicle.Register"/>), así que las reglas de la flota
    /// también se aplican a los datos de demostración, y las fechas son relativas a hoy, así que al cargarse nunca superan la antigüedad máxima.
    /// Es idempotente (se omite una matrícula que ya existe) y se ejecuta en segundo plano sin llegar a
    /// detener la aplicación: si la base de datos no está disponible, solo registra un aviso.
    /// </summary>
    internal sealed class DemoFleetSeeder(IServiceScopeFactory scopeFactory, TimeProvider timeProvider) : BackgroundService
    {
        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Loading demo data must never stop the application, whatever the reason it fails.")]
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = scopeFactory.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<IAppLogger<DemoFleetSeeder>>();

            try
            {
                var vehicles = scope.ServiceProvider.GetRequiredService<IVehicleRepository>();
                var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

                var added = 0;
                added += await Add(vehicles, today, "1111-AAA", "Seat", "Ibiza", 1);
                added += await Add(vehicles, today, "2222-BBB", "Seat", "Leon", 2);
                added += await Add(vehicles, today, "3333-CCC", "Toyota", "Corolla", 3);
                added += await Add(vehicles, today, "4444-DDD", "Renault", "Clio", 4);
                added += await Add(vehicles, today, "5555-EEE", "Volkswagen", "Golf", 2);

                logger.LogInformation("Flota de demostración lista: {added} vehículos nuevos cargados.", added);
            }
            catch (Exception ex)
            {
                logger.LogWarning("No se pudo cargar la flota de demostración: {message}", ex.Message);
            }
        }

        private static async Task<int> Add(IVehicleRepository vehicles, DateOnly today, string licensePlate, string brand, string model, int ageInYears)
        {
            var vehicle = Vehicle.Register(LicensePlate.Create(licensePlate), brand, model, today.AddYears(-ageInYears), today);

            try
            {
                await vehicles.Add(vehicle);
                return 1;
            }
            catch (LicensePlateAlreadyRegisteredException)
            {
                // Ya la cargó una ejecución anterior: no hay nada que hacer.
                return 0;
            }
        }
    }
}
