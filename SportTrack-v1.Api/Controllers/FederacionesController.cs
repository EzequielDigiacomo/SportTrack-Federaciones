using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Controladores.Federaciones;
using SportTrack_v1.Controladores.Federaciones.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportTrack_v1.Api.Controllers.Federaciones
{
    [ApiController]
    [Route("api/[controller]")]
    public class FederacionesController : ControllerBase
    {
        private readonly IFederacionServices _federacionServices;

        public FederacionesController(IFederacionServices federacionServices)
        {
            _federacionServices = federacionServices;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FederacionDto>>> GetFederaciones()
        {
            var result = await _federacionServices.GetAllFederacionesAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FederacionDto>> GetFederacion(int id)
        {
            var result = await _federacionServices.GetFederacionByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<FederacionDto>> CreateFederacion(FederacionCreateDto federacionDto)
        {
            var result = await _federacionServices.CreateFederacionAsync(federacionDto);
            return CreatedAtAction(nameof(GetFederacion), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<ActionResult<FederacionDto>> UpdateFederacion(int id, FederacionUpdateDto federacionDto)
        {
            var result = await _federacionServices.UpdateFederacionAsync(id, federacionDto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteFederacion(int id)
        {
            await _federacionServices.DeleteFederacionAsync(id);
            return NoContent();
        }
    }
}
