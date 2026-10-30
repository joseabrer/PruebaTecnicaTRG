using MediatR;

namespace GtMotive.Estimate.Microservice.Api.UseCases.ListAvailableVehicles
{
    /// <summary>
    /// Lista los vehículos que se pueden alquilar ahora mismo.
    /// </summary>
    public sealed class ListarVehiculosDisponiblesSolicitud : IRequest<IWebApiPresenter>
    {
    }
}
