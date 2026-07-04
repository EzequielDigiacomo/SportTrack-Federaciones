using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Entidades.DTOs.Entrenador;
using SportTrack_v1.Entidades.DTOs.Participante;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Federaciones
{
    public interface IEntrenadorServices
    {
        Task<ActionResult<EntrenadorDetailDto>> GetEntrenador(int id);
        Task<ActionResult<IEnumerable<EntrenadorDto>>> GetEntrenadores();
        Task<ActionResult<IEnumerable<EntrenadorDto>>> GetEntrenadoresPorClub(int idClub);
        Task<ActionResult<IEnumerable<EntrenadorDto>>> GetEntrenadoresSeleccion();
        Task<ActionResult<IEnumerable<EntrenadorDto>>> SearchEntrenadores(string term);
        Task<ActionResult<EntrenadorDto>> PostEntrenador(EntrenadorCreateDto entrenadorCreateDto);
        Task<IActionResult> PutEntrenador(int id, EntrenadorCreateDto entrenadorCreateDto);
        Task<IActionResult> DeleteEntrenador(int id);
    }
}
