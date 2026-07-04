using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Controladores.Federaciones;
using SportTrack_v1.Entidades.DTOs.Federacion;
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
            return await _federacionServices.GetFederaciones();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FederacionDto>> GetFederacion(int id)
        {
            return await _federacionServices.GetFederacion(id);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<FederacionDto>> CreateFederacion(FederacionCreateDto federacionDto)
        {
            return await _federacionServices.PostFederacion(federacionDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> UpdateFederacion(int id, FederacionCreateDto federacionDto)
        {
            return await _federacionServices.PutFederacion(id, federacionDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteFederacion(int id)
        {
            return await _federacionServices.DeleteFederacion(id);
        }
    }
}
