using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Controladores.SaaS;
using System.Threading.Tasks;
using System.Linq;

namespace SportTrack_v1.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SaaSController : ControllerBase
    {
        private readonly ISaaSService _saasService;

        public SaaSController(ISaaSService saasService)
        {
            _saasService = saasService;
        }

        [HttpGet("debug-me")]
        [AllowAnonymous]
        public ActionResult DebugMe()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            var identity = User.Identity;
            return Ok(new {
                IdentityName = identity?.Name,
                IsAuthenticated = identity?.IsAuthenticated,
                AuthenticationType = identity?.AuthenticationType,
                Claims = claims
            });
        }

        [HttpGet("planes")]
        public async Task<IActionResult> GetPlanes()
        {
            var planes = await _saasService.GetPlanesAsync();
            return Ok(planes);
        }

        [HttpPost("asignar-plan")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> AsignarPlan(int clubId, int planId)
        {
            await _saasService.AsignarPlanAClubAsync(clubId, planId);
            return Ok(new { message = "Plan asignado correctamente" });
        }

        [HttpGet("clubes-status")]
        [Authorize(Roles = "SuperAdmin,Admin,soporte_tecnico")]
        public async Task<IActionResult> GetClubesStatus()
        {
            var clubesStatus = await _saasService.GetClubesStatusAsync();
            return Ok(clubesStatus);
        }

        [HttpPatch("clubes/{id}/toggle-activo")]
        [Authorize(Roles = "SuperAdmin,Admin,soporte_tecnico")]
        public async Task<IActionResult> ToggleActivo(int id)
        {
            await _saasService.ToggleClubActivoAsync(id);
            return Ok(new { message = "Estado de federaciÃ³n actualizado correctamente" });
        }

        [HttpPost("create-federacion")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateFederacion([FromBody] SportTrack_v1.Controladores.SaaS.Dtos.SaaSCreateFederacionDto dto)
        {
            var id = await _saasService.CreateFederacionWithAdminAsync(dto);
            return Ok(new { id, message = "FederaciÃ³n y administrador creados correctamente" });
        }

        [HttpGet("global-metrics")]
        [Authorize(Roles = "SuperAdmin,soporte_tecnico")]
        public async Task<IActionResult> GetGlobalMetrics()
        {
            var metrics = await _saasService.GetGlobalMetricsAsync();
            return Ok(metrics);
        }
    }
}
