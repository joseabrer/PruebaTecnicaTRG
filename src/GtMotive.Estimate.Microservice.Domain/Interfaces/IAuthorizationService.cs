using System.Security.Claims;
using System.Threading.Tasks;

namespace GtMotive.Estimate.Microservice.Domain.Interfaces
{
    /// <summary>
    /// Comprueba los permisos basados en políticas de un usuario.
    /// </summary>
    public interface IAuthorizationService
    {
        /// <summary>
        /// Comprueba si un usuario cumple una política de autorización concreta.
        /// </summary>
        /// <param name="user">El usuario contra el que se comprueba la política.</param>
        /// <param name="resource">Un recurso opcional con el que se debe comprobar la política. Si la evaluación de la política no necesita un recurso, se puede pasar null.</param>
        /// <param name="policyName">El nombre de la política que se comprueba en un contexto concreto.</param>
        /// <returns>Indica si el usuario, y el recurso opcional, cumplen la política. True cuando se cumple la política y false en caso contrario.</returns>
        Task<bool> Authorize(ClaimsPrincipal user, object resource, string policyName);
    }
}
