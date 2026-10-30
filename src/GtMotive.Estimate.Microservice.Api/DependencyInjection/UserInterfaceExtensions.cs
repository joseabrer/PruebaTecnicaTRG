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

            // Each presenter is registered both as itself and as the output port of its use case. They resolve to
            // the same scoped instance, so what the use case writes to the port is what the handler gets back.
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
