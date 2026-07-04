using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Entidades.DTOs.DelegadoClub;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Federaciones
{
    public interface IDelegadoClubServices
    {
        Task<ActionResult<DelegadoClubDetailDto>> GetDelegadoClub(int id);
        Task<ActionResult<IEnumerable<DelegadoClubDto>>> GetDelegadosClub();
        Task<ActionResult<IEnumerable<DelegadoClubDto>>> GetDelegadosPorFederacion(int idFederacion);
        Task<ActionResult<DelegadoClubDto>> PostDelegadoClub(DelegadoClubCreateDto delegadoClubCreateDto);
        Task<IActionResult> PutDelegadoClub(int id, DelegadoClubCreateDto delegadoClubCreateDto);
        Task<IActionResult> DeleteDelegadoClub(int id);
    }
}
