using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Entidades.DTOs.AtletaTutor;
using SportTrack_v1.Controladores.Federaciones;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIGDEF.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AtletaTutorController : ControllerBase
    {
        private readonly IAtletaTutorServices _atletaTutorServices;

        public AtletaTutorController(IAtletaTutorServices atletaTutorServices)
        {
            _atletaTutorServices = atletaTutorServices;
        }

        // GET: api/AtletaTutor
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AtletaTutorDetailDto>>> GetAtletaTutores()
        {
            return await _atletaTutorServices.GetAtletaTutores();
        }

        // GET: api/AtletaTutor/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AtletaTutorDetailDto>> GetAtletaTutor(int id)
        {
            return await _atletaTutorServices.GetAtletaTutor(id);
        }

        // POST: api/AtletaTutor
        [HttpPost]
        public async Task<ActionResult<AtletaTutorDto>> PostAtletaTutor(AtletaTutorCreateDto atletaTutorCreateDto)
        {
            return await _atletaTutorServices.PostAtletaTutor(atletaTutorCreateDto);
        }

        // DELETE: api/AtletaTutor/5 (por ID de relaciÃ³n)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAtletaTutor(int id)
        {
            return await _atletaTutorServices.DeleteAtletaTutor(id);
        }

        // DELETE: api/AtletaTutor/5/10 (por ID de atleta e ID de tutor)
        [HttpDelete("{idAtleta}/{idTutor}")]
        public async Task<IActionResult> DeleteAtletaTutor(int idAtleta, int idTutor)
        {
            return await _atletaTutorServices.DeleteAtletaTutor(idAtleta, idTutor);
        }
    }
}
