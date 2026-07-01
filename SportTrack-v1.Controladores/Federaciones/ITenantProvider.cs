using System.Security.Claims;

namespace SportTrack_v1.Controladores.Federaciones
{
    public interface ITenantProvider
    {
        int? GetFederacionId();
        int? GetClubId();
        string GetRol();
        ClaimsPrincipal GetUser();
    }
}
