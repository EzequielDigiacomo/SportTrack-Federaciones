using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Entidades.DTOs.Inscripcion;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Federaciones
{
    public interface IInscripcionServices
    {
        Task<ActionResult<InscripcionDetailDto>> GetInscripcion(int id);
        Task<ActionResult<IEnumerable<InscripcionDto>>> GetInscripciones();
        Task<ActionResult<IEnumerable<InscripcionDto>>> GetInscripcionesPorAtleta(int ParticipanteId);
        Task<ActionResult<IEnumerable<InscripcionDto>>> GetInscripcionesPorEvento(int idEvento);
        Task<ActionResult<InscripcionDto>> PostInscripcion(InscripcionCreateDto inscripcionCreateDto);
        Task<IActionResult> PutInscripcion(int id, InscripcionCreateDto inscripcionCreateDto);
        Task<IActionResult> DeleteInscripcion(int id);
    }
}
