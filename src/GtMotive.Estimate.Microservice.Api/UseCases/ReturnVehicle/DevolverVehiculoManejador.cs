using System;
using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ReturnVehicle;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.ReturnVehicle
{
    public sealed class DevolverVehiculoManejador(
        IUseCase<ReturnVehicleInput> casoDeUso,
        DevolverVehiculoPresentador presentador) : IRequestHandler<DevolverVehiculoSolicitud, IWebApiPresenter>
    {
        private readonly IUseCase<ReturnVehicleInput> _casoDeUso = casoDeUso;
        private readonly DevolverVehiculoPresentador _presentador = presentador;

        public async Task<IWebApiPresenter> Handle(DevolverVehiculoSolicitud request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            await _casoDeUso.Execute(new ReturnVehicleInput(request.AlquilerId));

            return _presentador;
        }
    }
}
