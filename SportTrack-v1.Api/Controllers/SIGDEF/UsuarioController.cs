using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Entidades.DTOs.Usuario;
using SportTrack_v1.Controladores.Federaciones;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIGDEF.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioServices _usuarioServices;

        public UsuarioController(IUsuarioServices usuarioServices)
        {
            _usuarioServices = usuarioServices;
        }

        // GET: api/Usuario
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioResponseDto>>> GetUsuarios()
        {
            return await _usuarioServices.GetUsuarios();
        }

        // GET: api/Usuario/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioResponseDto>> GetUsuario(int id)
        {
            return await _usuarioServices.GetUsuario(id);
        }

        // POST: api/Usuario
        [HttpPost]
        public async Task<ActionResult<UsuarioResponseDto>> PostUsuario(UsuarioCreateDto usuarioCreateDto)
        {
            return await _usuarioServices.PostUsuario(usuarioCreateDto);
        }

        // PUT: api/Usuario/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, UsuarioUpdateDto usuarioUpdateDto)
        {
            return await _usuarioServices.PutUsuario(id, usuarioUpdateDto);
        }

        // DELETE: api/Usuario/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            return await _usuarioServices.DeleteUsuario(id);
        }

        // POST: api/Usuario/5/change-password
        [HttpPost("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(int id, UsuariochangePasswordDto usuariochangePasswordDto)
        {
            return await _usuarioServices.ChangePassword(id, usuariochangePasswordDto);
        }
    }
}
