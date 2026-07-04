using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Entidades.DTOs.Entrenador;
using SportTrack_v1.Controladores.Federaciones;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIGDEF.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntrenadorController : ControllerBase
    {
        private readonly IEntrenadorServices _entrenadorServices;

        public EntrenadorController(IEntrenadorServices entrenadorServices)
        {
            _entrenadorServices = entrenadorServices;
        }

        // GET: api/Entrenador
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntrenadorDetailDto>>> GetEntrenadores()
        {
            return await _entrenadorServices.GetEntrenadores();
        }

        // GET: api/Entrenador/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EntrenadorDetailDto>> GetEntrenador(int id)
        {
            return await _entrenadorServices.GetEntrenador(id);
        }

        // GET: api/Entrenador/seleccion
        [HttpGet("seleccion")]
        public async Task<ActionResult<IEnumerable<EntrenadorDto>>> GetEntrenadoresSeleccion()
        {
            return await _entrenadorServices.GetEntrenadoresSeleccion();
        }

        // POST: api/Entrenador
        [HttpPost]
        public async Task<ActionResult<EntrenadorDto>> PostEntrenador(EntrenadorCreateDto entrenadorCreateDto)
        {
            return await _entrenadorServices.PostEntrenador(entrenadorCreateDto);
        }

        // PUT: api/Entrenador/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEntrenador(int id, EntrenadorCreateDto entrenadorCreateDto)
        {
            return await _entrenadorServices.PutEntrenador(id, entrenadorCreateDto);
        }

        // DELETE: api/Entrenador/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEntrenador(int id)
        {
            return await _entrenadorServices.DeleteEntrenador(id);
        }
    }
}
