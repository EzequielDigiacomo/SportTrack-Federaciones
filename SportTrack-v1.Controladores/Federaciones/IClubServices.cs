using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Entidades.DTOs.Club;
using SportTrack_v1.Entidades.DTOs.AtletaFederado;
using SportTrack_v1.Entidades.DTOs.Entrenador;
using SportTrack_v1.Entidades.DTOs.DelegadoClub;
using SportTrack_v1.Entidades.DTOs.Evento;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Federaciones
{
    public interface IClubServices
    {
        Task<ActionResult<ClubDetailDto>> GetClub(int id);
        Task<ActionResult<IEnumerable<ClubDto>>> GetClubes();
        Task<ActionResult<IEnumerable<AtletaDto>>> GetAtletasByClub(int id);
        Task<ActionResult<IEnumerable<EntrenadorDto>>> GetEntrenadoresByClub(int id);
        Task<ActionResult<IEnumerable<DelegadoClubDto>>> GetDelegadosByClub(int id);
        Task<ActionResult<IEnumerable<EventoDto>>> GetEventosByClub(int id);
        Task<ActionResult<IEnumerable<ClubDto>>> SearchClubes(string term);
        Task<ActionResult<ClubDto>> PostClub(ClubCreateDto clubCreateDto);
        Task<IActionResult> PutClub(int id, ClubCreateDto clubCreateDto);
        Task<IActionResult> DeleteClub(int id);
    }
}
