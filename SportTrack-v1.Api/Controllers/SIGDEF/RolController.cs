using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Entidades.DTOs.Rol;
using SportTrack_v1.Controladores.Federaciones;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIGDEF.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolController : ControllerBase
    {
        private readonly IRolServices _rolServices;

        public RolController(IRolServices rolServices)
        {
            _rolServices = rolServices;
        }

        // GET: api/Rol
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RolDetailDto>>> GetRoles()
        {
            return await _rolServices.GetRoles();
        }

        // GET: api/Rol/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RolDetailDto>> GetRol(int id)
        {
            return await _rolServices.GetRol(id);
        }

        // POST: api/Rol
        [HttpPost]
        public async Task<ActionResult<RolDto>> PostRol(RolCreateDto rolCreateDto)
        {
            return await _rolServices.PostRol(rolCreateDto);
        }

        // PUT: api/Rol/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRol(int id, RolCreateDto rolCreateDto)
        {
            return await _rolServices.PutRol(id, rolCreateDto);
        }

        // DELETE: api/Rol/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRol(int id)
        {
            return await _rolServices.DeleteRol(id);
        }
    }
}
