using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Entidades.DTOs.Rol;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Federaciones
{
    public interface IRolServices
    {
        Task<ActionResult<RolDetailDto>> GetRol(int id);
        Task<ActionResult<IEnumerable<RolDto>>> GetRoles();
        Task<ActionResult<IEnumerable<RolDto>>> GetRolesPredefinidos();
        Task<ActionResult<IEnumerable<RolDto>>> SearchRoles(string term);
        Task<ActionResult<RolDto>> GetRolPorTipo(string tipo);
        Task<ActionResult<RolDto>> GetRolPorEnumId(int enumId);
        Task<ActionResult> GetEnumValues();
        Task<ActionResult<RolDto>> PostRol(RolCreateDto rolCreateDto);
        Task<IActionResult> PutRol(int id, RolCreateDto rolCreateDto);
        Task<IActionResult> DeleteRol(int id);
    }
}
