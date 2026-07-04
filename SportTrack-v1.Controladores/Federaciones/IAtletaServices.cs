using Microsoft.AspNetCore.Mvc;
using SIGDEF.DTOs;
using SportTrack_v1.Entidades.DTOs.AtletaFederado;
using SportTrack_v1.Entidades.DTOs.Base;

namespace SportTrack_v1.Controladores.Federaciones
{
    public interface IAtletaServices
    {
        Task<IActionResult> DeleteAtleta(int id);
        Task<ActionResult<AtletaDetailDto>> GetAtleta(int id);
        Task<ActionResult<IEnumerable<AtletaDetailDto>>> GetAtletas();
        Task<ActionResult<PagedResponseDto<AtletaListDto>>> GetAtletasPaginadosAsync(PaginationParamsDto parameters);
        Task<ActionResult<AtletaDto>> PostAtleta(AtletaCreateDto atletaCreateDto);
        Task<ActionResult<AtletaDto>> PostAtletaFull(AtletaFullCreateDto atletaFullCreateDto);
        Task<IActionResult> PutAtleta(int id, AtletaCreateDto atletaCreateDto);
    }
}
