using System;
using GtMotive.Estimate.Microservice.Api.UseCases.CreateVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.ListAvailableVehicles;
using GtMotive.Estimate.Microservice.Api.UseCases.RentVehicle;
using GtMotive.Estimate.Microservice.Api.UseCases.ReturnVehicle;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.CreateVehicle;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ListAvailableVehicles;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ReturnVehicle;
using Microsoft.Extensions.DependencyInjection;

namespace GtMotive.Estimate.Microservice.Api.DependencyInjection
{
    public static class UserInterfaceExtensions
    {
        public static IServiceCollection AddPresenters(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            // Cada presenter se registra a la vez como él mismo y como el puerto de salida de su caso de uso. Los dos se resuelven a
            // la misma instancia scoped, así que lo que el caso de uso escribe en el puerto es lo que recibe el manejador.
            services.AddScoped<CrearVehiculoPresentador>();
            services.AddScoped<ICreateVehicleOutputPort>(provider => provider.GetRequiredService<CrearVehiculoPresentador>());

            services.AddScoped<ListarVehiculosDisponiblesPresentador>();
            services.AddScoped<IListAvailableVehiclesOutputPort>(provider => provider.GetRequiredService<ListarVehiculosDisponiblesPresentador>());

            services.AddScoped<AlquilarVehiculoPresentador>();
            services.AddScoped<IRentVehicleOutputPort>(provider => provider.GetRequiredService<AlquilarVehiculoPresentador>());

            services.AddScoped<DevolverVehiculoPresentador>();
            services.AddScoped<IReturnVehicleOutputPort>(provider => provider.GetRequiredService<DevolverVehiculoPresentador>());

            return services;
        }
    }
}
