using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Entidades.DTOs.Inscripcion;
using SportTrack_v1.Controladores.Federaciones;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIGDEF.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InscripcionController : ControllerBase
    {
        private readonly IInscripcionServices _inscripcionServices;

        public InscripcionController(IInscripcionServices inscripcionServices)
        {
            _inscripcionServices = inscripcionServices;
        }

        // GET: api/Inscripcion
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InscripcionDetailDto>>> GetInscripciones()
        {
            return await _inscripcionServices.GetInscripciones();
        }

        // GET: api/Inscripcion/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InscripcionDetailDto>> GetInscripcion(int id)
        {
            return await _inscripcionServices.GetInscripcion(id);
        }

        // GET: api/Inscripcion/evento/5
        [HttpGet("evento/{idEvento}")]
        public async Task<ActionResult<IEnumerable<InscripcionDetailDto>>> GetInscripcionesPorEvento(int idEvento)
        {
            return await _inscripcionServices.GetInscripcionesPorEvento(idEvento);
        }

        // POST: api/Inscripcion
        [HttpPost]
        public async Task<ActionResult<InscripcionDto>> PostInscripcion(InscripcionCreateDto inscripcionCreateDto)
        {
            return await _inscripcionServices.PostInscripcion(inscripcionCreateDto);
        }

        // DELETE: api/Inscripcion/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInscripcion(int id)
        {
            return await _inscripcionServices.DeleteInscripcion(id);
        }
    }
}
