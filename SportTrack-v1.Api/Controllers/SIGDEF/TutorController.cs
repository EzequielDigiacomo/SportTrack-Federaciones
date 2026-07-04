using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Entidades.DTOs.Tutor;
using SportTrack_v1.Controladores.Federaciones;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIGDEF.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TutorController : ControllerBase
    {
        private readonly ITutorServices _tutorServices;

        public TutorController(ITutorServices tutorServices)
        {
            _tutorServices = tutorServices;
        }

        // GET: api/Tutor
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TutorDetailDto>>> GetTutores()
        {
            return await _tutorServices.GetTutores();
        }

        // GET: api/Tutor/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TutorDetailDto>> GetTutor(int id)
        {
            return await _tutorServices.GetTutor(id);
        }

        // POST: api/Tutor
        [HttpPost]
        public async Task<ActionResult<TutorDto>> PostTutor(TutorCreateDto tutorCreateDto)
        {
            return await _tutorServices.PostTutor(tutorCreateDto);
        }

        // PUT: api/Tutor/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTutor(int id, TutorCreateDto tutorCreateDto)
        {
            return await _tutorServices.PutTutor(id, tutorCreateDto);
        }

        // DELETE: api/Tutor/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTutor(int id)
        {
            return await _tutorServices.DeleteTutor(id);
        }
    }
}
