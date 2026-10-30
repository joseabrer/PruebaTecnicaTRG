using System;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.RentVehicle;
using GtMotive.Estimate.Microservice.Domain;
using GtMotive.Estimate.Microservice.Domain.Rentals;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.RentVehicle
{
    public sealed class AlquilarVehiculoManejador(
        IUseCase<RentVehicleInput> casoDeUso,
        AlquilarVehiculoPresentador presentador) : IRequestHandler<AlquilarVehiculoSolicitud, IWebApiPresenter>
    {
        private readonly IUseCase<RentVehicleInput> _casoDeUso = casoDeUso;
        private readonly AlquilarVehiculoPresentador _presentador = presentador;

        public async Task<IWebApiPresenter> Handle(AlquilarVehiculoSolicitud request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var entrada = new RentVehicleInput(
                request.VehiculoId ?? throw new DomainException("El identificador del vehículo es obligatorio."),
                RenterId.Create(request.ClienteId));

            await _casoDeUso.Execute(entrada);

            return _presentador;
        }
    }
}
