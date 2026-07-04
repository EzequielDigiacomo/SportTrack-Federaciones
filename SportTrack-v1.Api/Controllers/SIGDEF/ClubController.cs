using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Entidades.DTOs.Club;
using SportTrack_v1.Controladores.Federaciones;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIGDEF.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClubController : ControllerBase
    {
        private readonly IClubServices _clubServices;

        public ClubController(IClubServices clubServices)
        {
            _clubServices = clubServices;
        }

        // GET: api/Club
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClubDto>>> GetClubes()
        {
            return await _clubServices.GetClubes();
        }

        // GET: api/Club/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClubDetailDto>> GetClub(int id)
        {
            return await _clubServices.GetClub(id);
        }

        // GET: api/Club/search/boca
        [HttpGet("search/{term}")]
        public async Task<ActionResult<IEnumerable<ClubDto>>> SearchClubes(string term)
        {
            return await _clubServices.SearchClubes(term);
        }

        // POST: api/Club
        [HttpPost]
        public async Task<ActionResult<ClubDto>> PostClub(ClubCreateDto clubCreateDto)
        {
            return await _clubServices.PostClub(clubCreateDto);
        }

        // PUT: api/Club/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClub(int id, ClubCreateDto clubCreateDto)
        {
            return await _clubServices.PutClub(id, clubCreateDto);
        }

        // DELETE: api/Club/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClub(int id)
        {
            return await _clubServices.DeleteClub(id);
        }
    }
}
