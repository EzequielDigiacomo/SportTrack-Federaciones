using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Entidades.DTOs.Usuario;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Federaciones
{
    public interface IUsuarioServices
    {
        Task<ActionResult<UsuarioDetailDto>> GetUsuario(int id);
        Task<ActionResult<IEnumerable<UsuarioDto>>> GetUsuarios();
        Task<ActionResult<UsuarioDto>> GetUsuarioPorUsername(string username);
        Task<ActionResult<UsuarioDto>> PostUsuario(UsuarioCreateDto usuarioCreateDto);
        Task<ActionResult<UsuarioDto>> Login(UsuarioLoginDto loginDto);
        Task<IActionResult> PutUsuario(int id, UsuarioUpdateDto usuarioUpdateDto);
        Task<IActionResult> ChangePassword(int id, UsuarioChangePasswordDto changePasswordDto);
        Task<IActionResult> DeleteUsuario(int id);
        Task<ActionResult<string>> ResetPassword(int id);
    }
}
