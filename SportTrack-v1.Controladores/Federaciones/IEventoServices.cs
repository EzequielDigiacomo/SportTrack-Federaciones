using Microsoft.AspNetCore.Mvc;
using SIGDEF.DTOs;
using SportTrack_v1.Entidades.DTOs.Evento;
using SportTrack_v1.Entidades.DTOs.EventoPrueba;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Federaciones
{
    public interface IEventoServices
    {
        Task<ActionResult<EventoResponseDto>> GetEvento(int id);
        Task<ActionResult<EventoDetailDto>> GetEventoDetalle(int id);
        Task<ActionResult<IEnumerable<EventoDto>>> GetEventos(
            bool? activos = null,
            string? tipo = null,
            string? provincia = null,
            int? distancia = null);
        Task<ActionResult<IEnumerable<EventoDto>>> GetProximosEventos();
        Task<ActionResult<IEnumerable<EventoResponseDto>>> GetEventosConInscripcionesAbiertas();
        Task<ActionResult<IEnumerable<EventoResponseDto>>> GetEventosPorDistancia(int distancia);
        Task<ActionResult<EventoFormConfigDto>> GetFormConfig();
        Task<ActionResult<EventoResponseDto>> PostEvento(EventoCreateDTO eventoDto);
        Task<IActionResult> PutEvento(int id, EventoUpdateDto eventoDto);
        Task<IActionResult> ActivarEvento(int id);
        Task<IActionResult> DesactivarEvento(int id);
        Task<IActionResult> DeleteEvento(int id);
    }
}
