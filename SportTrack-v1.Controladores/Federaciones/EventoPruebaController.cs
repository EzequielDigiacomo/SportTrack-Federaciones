using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportTrack.AccessDatos;
using SportTrack_v1.Entidades.Entidades;
using SportTrack_v1.Entidades.DTOs.EventoPrueba;
using SportTrack_v1.Entidades.DTOs.Prueba;

namespace SIGDEF.Controllers
{
    [ApiController]
    [Route("api/eventos/{idEvento}/pruebas")]
    public class EventoPruebaController : ControllerBase
    {
        private readonly SportTrackDbContext _context;

        public EventoPruebaController(SportTrackDbContext context)
        {
            _context = context;
        }

        // GET: api/eventos/5/pruebas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventoPruebaDto>>> GetEventoPruebas(int idEvento)
        {
            var eventoExiste = await _context.Eventos.AnyAsync(e => e.IdEvento == idEvento);
            if (!eventoExiste)
            {
                return NotFound(new { message = $"Evento con ID {idEvento} no encontrado" });
            }

            return await _context.EventoPruebas
                .Include(ep => ep.Prueba)
                .Where(ep => ep.IdEvento == idEvento)
                .Select(ep => new EventoPruebaDto
                {
                    IdEventoPrueba = ep.IdEventoPrueba,
                    IdEvento = ep.IdEvento,
                    IdPrueba = ep.IdPrueba,
                    PrecioCategoria = ep.PrecioCategoria,
                    Prueba = new PruebaDto
                    {
                        IdPrueba = ep.Prueba.IdPrueba,
                        Distancia = ep.Prueba.Distancia,
                        CategoriaEdad = ep.Prueba.CategoriaEdad,
                        SexoCompetencia = ep.Prueba.SexoCompetencia,
                        TipoBote = ep.Prueba.TipoBote
                    }
                })
                .ToListAsync();
        }

        // GET: api/eventos/5/pruebas/3
        [HttpGet("{idPrueba}")]
        public async Task<ActionResult<EventoPruebaDto>> GetEventoPrueba(int idEvento, int idPrueba)
        {
            var eventoPrueba = await _context.EventoPruebas
                .Include(ep => ep.Prueba)
                .FirstOrDefaultAsync(ep => ep.IdEvento == idEvento && ep.IdEventoPrueba == idPrueba);

            if (eventoPrueba == null)
            {
                return NotFound(new
                {
                    message = $"Prueba con ID {idPrueba} no encontrada en el evento {idEvento}"
                });
            }

            return new EventoPruebaDto
            {
                IdEventoPrueba = eventoPrueba.IdEventoPrueba,
                IdEvento = eventoPrueba.IdEvento,
                IdPrueba = eventoPrueba.IdPrueba,
                PrecioCategoria = eventoPrueba.PrecioCategoria,
                Prueba = new PruebaDto
                {
                    IdPrueba = eventoPrueba.Prueba.IdPrueba,
                    Distancia = eventoPrueba.Prueba.Distancia,
                    CategoriaEdad = eventoPrueba.Prueba.CategoriaEdad,
                    SexoCompetencia = eventoPrueba.Prueba.SexoCompetencia,
                    TipoBote = eventoPrueba.Prueba.TipoBote
                }
            };
        }

        // POST: api/eventos/5/pruebas
        [HttpPost]
        public async Task<ActionResult<EventoPruebaDto>> PostEventoPrueba(
            int idEvento,
            EventoPruebaCreateDto eventoPruebaDto)
        {
            var evento = await _context.Eventos.FindAsync(idEvento);
            if (evento == null)
            {
                return NotFound(new { message = $"Evento con ID {idEvento} no encontrado" });
            }

            // Verificar si el IdPrueba existe en el catálogo
            var pruebaCatalog = await _context.Pruebas.FindAsync(eventoPruebaDto.IdPrueba);
            if (pruebaCatalog == null)
            {
                 return BadRequest(new { message = $"La prueba con ID {eventoPruebaDto.IdPrueba} no existe en el catálogo." });
            }

            // Verificar que no exista ya esa prueba en el evento
            var pruebaExistente = await _context.EventoPruebas
                .AnyAsync(ep => ep.IdEvento == idEvento && 
                                ep.IdPrueba == eventoPruebaDto.IdPrueba);

            if (pruebaExistente)
            {
                return BadRequest(new
                {
                    message = $"Esta prueba ya existe en este evento"
                });
            }

            var eventoPrueba = new EventoPrueba
            {
                IdEvento = idEvento,
                IdPrueba = eventoPruebaDto.IdPrueba,
                PrecioCategoria = eventoPruebaDto.PrecioCategoria
            };

            _context.EventoPruebas.Add(eventoPrueba);
            await _context.SaveChangesAsync();

            // Recargar para incluir navegación
            await _context.Entry(eventoPrueba).Reference(ep => ep.Prueba).LoadAsync();

            var resultDto = new EventoPruebaDto
            {
                IdEventoPrueba = eventoPrueba.IdEventoPrueba,
                IdEvento = eventoPrueba.IdEvento,
                IdPrueba = eventoPrueba.IdPrueba,
                PrecioCategoria = eventoPrueba.PrecioCategoria,
                Prueba = new PruebaDto
                {
                    IdPrueba = eventoPrueba.Prueba.IdPrueba,
                    Distancia = eventoPrueba.Prueba.Distancia,
                    CategoriaEdad = eventoPrueba.Prueba.CategoriaEdad,
                    SexoCompetencia = eventoPrueba.Prueba.SexoCompetencia,
                    TipoBote = eventoPrueba.Prueba.TipoBote
                }
            };

            return CreatedAtAction(
                nameof(GetEventoPrueba),
                new { idEvento = idEvento, idPrueba = eventoPrueba.IdEventoPrueba },
                resultDto);
        }

        // PUT: api/eventos/5/pruebas/3
        [HttpPut("{idPrueba}")]
        public async Task<IActionResult> PutEventoPrueba(
            int idEvento,
            int idPrueba,
            EventoPruebaUpdateDto eventoPruebaDto)
        {
            if (idPrueba != eventoPruebaDto.IdEventoPrueba)
            {
                return BadRequest(new { message = "El ID de la ruta no coincide con el ID de la prueba" });
            }

            var eventoPrueba = await _context.EventoPruebas
                .FirstOrDefaultAsync(ep => ep.IdEvento == idEvento && ep.IdEventoPrueba == idPrueba);

            if (eventoPrueba == null)
            {
                return NotFound(new
                {
                    message = $"Prueba con ID {idPrueba} no encontrada en el evento {idEvento}"
                });
            }

            // Verificar catálogo si cambió el ID
            if (eventoPruebaDto.IdPrueba != eventoPrueba.IdPrueba) {
                 var pruebaCatalog = await _context.Pruebas.FindAsync(eventoPruebaDto.IdPrueba);
                 if (pruebaCatalog == null)
                 {
                      return BadRequest(new { message = $"La prueba con ID {eventoPruebaDto.IdPrueba} no existe en el catálogo." });
                 }
                 eventoPrueba.IdPrueba = eventoPruebaDto.IdPrueba;
            }

            eventoPrueba.PrecioCategoria = eventoPruebaDto.PrecioCategoria;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventoPruebaExists(idEvento, idPrueba))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/eventos/5/pruebas/3
        [HttpDelete("{idPrueba}")]
        public async Task<IActionResult> DeleteEventoPrueba(int idEvento, int idPrueba)
        {
            // Nota: aquí idPrueba es IdEventoPrueba (PK de la intermedia), no el IdPrueba del catálogo
            var eventoPrueba = await _context.EventoPruebas
                .FirstOrDefaultAsync(ep => ep.IdEvento == idEvento && ep.IdEventoPrueba == idPrueba);

            if (eventoPrueba == null)
            {
                return NotFound(new
                {
                    message = $"Prueba con ID {idPrueba} no encontrada en el evento {idEvento}"
                });
            }

            _context.EventoPruebas.Remove(eventoPrueba);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EventoPruebaExists(int idEvento, int idPrueba)
        {
            return _context.EventoPruebas
                .Any(ep => ep.IdEvento == idEvento && ep.IdEventoPrueba == idPrueba);
        }
    }
}
