using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Entidades.DTOs.DelegadoClub;
using SportTrack_v1.Controladores.Federaciones;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIGDEF.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DelegadoClubController : ControllerBase
    {
        private readonly IDelegadoClubServices _delegadoClubServices;

        public DelegadoClubController(IDelegadoClubServices delegadoClubServices)
        {
            _delegadoClubServices = delegadoClubServices;
        }

        // GET: api/DelegadoClub
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DelegadoClubDto>>> GetDelegadosClub()
        {
            return await _delegadoClubServices.GetDelegadosClub();
        }

        // GET: api/DelegadoClub/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DelegadoClubDetailDto>> GetDelegadoClub(int id)
        {
            return await _delegadoClubServices.GetDelegadoClub(id);
        }

        // GET: api/DelegadoClub/federacion/1
        [HttpGet("federacion/{idFederacion}")]
        public async Task<ActionResult<IEnumerable<DelegadoClubDto>>> GetDelegadosPorFederacion(int idFederacion)
        {
            return await _delegadoClubServices.GetDelegadosPorFederacion(idFederacion);
        }

        // POST: api/DelegadoClub
        [HttpPost]
        public async Task<ActionResult<DelegadoClubDto>> PostDelegadoClub(DelegadoClubCreateDto delegadoClubCreateDto)
        {
            return await _delegadoClubServices.PostDelegadoClub(delegadoClubCreateDto);
        }

        // PUT: api/DelegadoClub/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDelegadoClub(int id, DelegadoClubCreateDto delegadoClubCreateDto)
        {
            return await _delegadoClubServices.PutDelegadoClub(id, delegadoClubCreateDto);
        }

        // DELETE: api/DelegadoClub/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDelegadoClub(int id)
        {
            return await _delegadoClubServices.DeleteDelegadoClub(id);
        }
    }
}
