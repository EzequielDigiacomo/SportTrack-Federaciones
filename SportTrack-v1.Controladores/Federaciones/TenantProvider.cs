using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace SportTrack_v1.Controladores.Federaciones
{
    public class TenantProvider : ITenantProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal GetUser()
        {
            return _httpContextAccessor.HttpContext?.User;
        }

        public int? GetFederacionId()
        {
            var user = GetUser();
            if (user == null) return null;

            var federacionIdClaim = user.FindFirst("FederacionId")?.Value;
            if (int.TryParse(federacionIdClaim, out int federacionId))
            {
                return federacionId;
            }

            return null;
        }

        public int? GetClubId()
        {
            var user = GetUser();
            if (user == null) return null;

            var clubIdClaim = user.FindFirst("ClubId")?.Value;
            if (int.TryParse(clubIdClaim, out int clubId))
            {
                return clubId;
            }

            return null;
        }

        public string GetRol()
        {
            var user = GetUser();
            return user?.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
        }
    }
}
