using System.Threading;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases;
using GtMotive.Estimate.Microservice.ApplicationCore.UseCases.ListAvailableVehicles;
using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.ListAvailableVehicles
{
    public sealed class ListarVehiculosDisponiblesManejador(
        IUseCase<ListAvailableVehiclesInput> casoDeUso,
        ListarVehiculosDisponiblesPresentador presentador) : IRequestHandler<ListarVehiculosDisponiblesSolicitud, IWebApiPresenter>
    {
        private readonly IUseCase<ListAvailableVehiclesInput> _casoDeUso = casoDeUso;
        private readonly ListarVehiculosDisponiblesPresentador _presentador = presentador;

        public async Task<IWebApiPresenter> Handle(ListarVehiculosDisponiblesSolicitud request, CancellationToken cancellationToken)
        {
            await _casoDeUso.Execute(new ListAvailableVehiclesInput());

            return _presentador;
        }
    }
}
