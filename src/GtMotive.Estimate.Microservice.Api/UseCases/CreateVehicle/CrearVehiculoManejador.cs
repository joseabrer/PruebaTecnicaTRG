using System;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.CreateVehicle;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Vehicles;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.CreateVehicle
{
    public sealed class CrearVehiculoManejador(
        IUseCase<CreateVehicleInput> casoDeUso,
        CrearVehiculoPresentador presentador) : IRequestHandler<CrearVehiculoSolicitud, IWebApiPresenter>
    {
        private readonly IUseCase<CreateVehicleInput> _casoDeUso = casoDeUso;
        private readonly CrearVehiculoPresentador _presentador = presentador;

        public async Task<IWebApiPresenter> Handle(CrearVehiculoSolicitud request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var entrada = new CreateVehicleInput(
                LicensePlate.Create(request.Matricula),
                request.Marca,
                request.Modelo,
                request.FechaFabricacion ?? throw new DomainException("La fecha de fabricación es obligatoria."));

            await _casoDeUso.Execute(entrada);

            return _presentador;
        }
    }
}
