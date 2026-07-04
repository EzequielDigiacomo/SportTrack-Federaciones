using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Entidades.DTOs.Participante;
using SportTrack_v1.Controladores.Federaciones;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIGDEF.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonaController : ControllerBase
    {
        private readonly IPersonaServices _personaServices;

        public PersonaController(IPersonaServices personaServices)
        {
            _personaServices = personaServices;
        }

        // GET: api/Persona
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonaDto>>> GetPersonas()
        {
            return await _personaServices.GetPersonas();
        }

        // GET: api/Persona/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PersonaDetailDto>> GetPersona(int id)
        {
            return await _personaServices.GetPersona(id);
        }

        // GET: api/Persona/documento/12345678
        [HttpGet("documento/{documento}")]
        public async Task<ActionResult<PersonaDto>> GetPersonaByDocumento(string documento)
        {
            return await _personaServices.GetPersonaByDocumento(documento);
        }

        // POST: api/Persona
        [HttpPost]
        public async Task<ActionResult<PersonaDto>> PostPersona(PersonaCreateDto personaCreateDto)
        {
            return await _personaServices.PostPersona(personaCreateDto);
        }

        // PUT: api/Persona/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPersona(int id, PersonaCreateDto personaCreateDto)
        {
            return await _personaServices.PutPersona(id, personaCreateDto);
        }

        // DELETE: api/Persona/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePersona(int id)
        {
            return await _personaServices.DeletePersona(id);
        }
    }
}
