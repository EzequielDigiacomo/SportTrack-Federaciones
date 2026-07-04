using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Entidades.DTOs.Tutor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Federaciones
{
    public interface ITutorServices
    {
        Task<ActionResult<TutorDetailDto>> GetTutor(int id);
        Task<ActionResult<IEnumerable<TutorDto>>> GetTutores();
        Task<ActionResult<IEnumerable<TutorDto>>> GetTutoresPorTipo(string tipoTutor);
        Task<ActionResult<IEnumerable<TutorDto>>> GetTutoresSinAtletas();
        Task<ActionResult<IEnumerable<string>>> GetTiposTutor();
        Task<ActionResult<IEnumerable<TutorDto>>> SearchTutores(string term);
        Task<ActionResult<TutorDto>> PostTutor(TutorCreateDto tutorCreateDto);
        Task<IActionResult> PutTutor(int id, TutorCreateDto tutorCreateDto);
        Task<IActionResult> DeleteTutor(int id);
    }
}
