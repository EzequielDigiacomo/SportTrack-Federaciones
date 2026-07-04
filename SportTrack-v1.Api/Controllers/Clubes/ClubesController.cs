using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Controladores.Club;
using SportTrack_v1.Controladores.Club.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportTrack_v1.Api.Controllers.Clubes
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClubesController : ControllerBase
    {
        private readonly IClubService _clubService;
        private readonly SportTrack_v1.Controladores.Federaciones.IClubServices _clubServicesSigdef;

        public ClubesController(IClubService clubService, SportTrack_v1.Controladores.Federaciones.IClubServices clubServicesSigdef)
        {
            _clubService = clubService;
            _clubServicesSigdef = clubServicesSigdef;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClubDto>>> GetClubes()
        {
            var result = await _clubService.GetAllClubesAsync();

            // Filtrar segÃºn el rol del usuario logueado
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var clubIdClaim = User.FindFirst("ClubId")?.Value;

            // Si es SuperAdmin ve todo
            if (role == "SuperAdmin")
                return Ok(result);

            // Si es Admin, solo ve los clubes de su federaciÃ³n (FederacionId == su FederacionId)
            // Ojo, en el nuevo modelo el rol Admin (federaciÃ³n) tiene su propio FederacionId en el claim
            var fedIdClaim = User.FindFirst("FederacionId")?.Value;
            if (role == "Admin" && int.TryParse(fedIdClaim, out int fedId))
            {
                var filtered = result.Where(c => c.FederacionId == fedId);
                return Ok(filtered);
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClubDto>> GetClub(int id)
        {
            var result = await _clubService.GetClubByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ClubDto>> CreateClub(ClubCreateDto clubDto)
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (role == "Admin")
            {
                var fedIdClaim = User.FindFirst("FederacionId")?.Value;
                if (int.TryParse(fedIdClaim, out int fedId) && fedId > 0)
                {
                    clubDto.FederacionId = fedId;
                }
            }
            var result = await _clubService.CreateClubAsync(clubDto);
            return CreatedAtAction(nameof(GetClub), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        // [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ClubDto>> UpdateClub(int id, ClubUpdateDto clubDto)
        {
            var result = await _clubService.UpdateClubAsync(id, clubDto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteClub(int id)
        {
            await _clubService.DeleteClubAsync(id);
            return NoContent();
        }

        // Sub-rutas requeridas por SIGDEF (vÃ­a _clubServicesSigdef)
        [HttpGet("{id}/Atletas")]
        public async Task<ActionResult<IEnumerable<SportTrack_v1.Entidades.DTOs.AtletaFederado.AtletaDto>>> GetAtletasByClub(int id)
        {
            return await _clubServicesSigdef.GetAtletasByClub(id);
        }

        [HttpGet("{id}/Entrenadores")]
        public async Task<ActionResult<IEnumerable<SportTrack_v1.Entidades.DTOs.Entrenador.EntrenadorDto>>> GetEntrenadoresByClub(int id)
        {
            return await _clubServicesSigdef.GetEntrenadoresByClub(id);
        }

        [HttpGet("{id}/Delegados")]
        public async Task<ActionResult<IEnumerable<SportTrack_v1.Entidades.DTOs.DelegadoClub.DelegadoClubDto>>> GetDelegadosByClub(int id)
        {
            return await _clubServicesSigdef.GetDelegadosByClub(id);
        }

        [HttpGet("{id}/Eventos")]
        public async Task<ActionResult<IEnumerable<SportTrack_v1.Entidades.DTOs.Evento.EventoDto>>> GetEventosByClub(int id)
        {
            return await _clubServicesSigdef.GetEventosByClub(id);
        }
    }
}
