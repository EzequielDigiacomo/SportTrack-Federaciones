using Microsoft.AspNetCore.Mvc;
using SIGDEF.DTOs;
using SportTrack_v1.Entidades.DTOs.AtletaFederado;
using SportTrack_v1.Entidades.DTOs.Base;
using SportTrack_v1.Controladores.Federaciones;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIGDEF.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AtletaController : ControllerBase
    {
        private readonly IAtletaServices _atletaServices;

        public AtletaController(IAtletaServices atletaServices)
        {
            _atletaServices = atletaServices;
        }

        // GET: api/Atleta
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AtletaDetailDto>>> GetAtletas()
        {
            return await _atletaServices.GetAtletas();
        }

        // GET: api/Atleta/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AtletaDetailDto>> GetAtleta(int id)
        {
            return await _atletaServices.GetAtleta(id);
        }

        // GET: api/Atleta/paged
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResponseDto<AtletaListDto>>> GetAtletasPaginados([FromQuery] PaginationParamsDto parameters)
        {
            return await _atletaServices.GetAtletasPaginadosAsync(parameters);
        }

        // POST: api/Atleta
        [HttpPost]
        public async Task<ActionResult<AtletaDto>> PostAtleta(AtletaCreateDto atletaCreateDto)
        {
            return await _atletaServices.PostAtleta(atletaCreateDto);
        }

        // POST: api/Atleta/full
        [HttpPost("full")]
        public async Task<ActionResult<AtletaDto>> PostAtletaFull(AtletaFullCreateDto atletaFullCreateDto)
        {
            return await _atletaServices.PostAtletaFull(atletaFullCreateDto);
        }

        // PUT: api/Atleta/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAtleta(int id, AtletaCreateDto atletaCreateDto)
        {
            return await _atletaServices.PutAtleta(id, atletaCreateDto);
        }

        // DELETE: api/Atleta/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAtleta(int id)
        {
            return await _atletaServices.DeleteAtleta(id);
        }
    }
}
