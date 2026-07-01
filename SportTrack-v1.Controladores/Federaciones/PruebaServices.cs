using Microsoft.EntityFrameworkCore;
using SportTrack.AccessDatos;
using SportTrack_v1.Entidades.Entidades;
using SportTrack_v1.Entidades.DTOs.Prueba;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SportTrack_v1.Controladores.Federaciones;
using Microsoft.AspNetCore.Mvc;

namespace SIGDEF.API.Services
{
    public class PruebaServices : IPruebaServices
    {
        private readonly SportTrackDbContext _context;

        public PruebaServices(SportTrackDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult<IEnumerable<PruebaDto>>> GetPruebas()
        {
            try
            {
                var pruebas = await _context.Pruebas
                    .Select(p => new PruebaDto
                    {
                        IdPrueba = p.IdPrueba,
                        Distancia = p.Distancia,
                        CategoriaEdad = p.CategoriaEdad,
                        SexoCompetencia = p.SexoCompetencia,
                        TipoBote = p.TipoBote
                    })
                    .ToListAsync();

                return new OkObjectResult(pruebas);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<PruebaDto>> GetPrueba(int id)
        {
            try
            {
                var prueba = await _context.Pruebas.FindAsync(id);

                if (prueba == null)
                {
                    return new NotFoundResult();
                }

                return new OkObjectResult(new PruebaDto
                {
                    IdPrueba = prueba.IdPrueba,
                    Distancia = prueba.Distancia,
                    CategoriaEdad = prueba.CategoriaEdad,
                    SexoCompetencia = prueba.SexoCompetencia,
                    TipoBote = prueba.TipoBote
                });
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<PruebaDto>> PostPrueba(PruebaCreateDto pruebaDto)
        {
            try
            {
                var existe = await _context.Pruebas.AnyAsync(p =>
                    p.Distancia == pruebaDto.Distancia &&
                    p.CategoriaEdad == pruebaDto.CategoriaEdad &&
                    p.SexoCompetencia == pruebaDto.SexoCompetencia &&
                    p.TipoBote == pruebaDto.TipoBote);

                if (existe)
                {
                    return new BadRequestResult();
                }

                var prueba = new Prueba
                {
                    Distancia = pruebaDto.Distancia,
                    CategoriaEdad = pruebaDto.CategoriaEdad,
                    SexoCompetencia = pruebaDto.SexoCompetencia,
                    TipoBote = pruebaDto.TipoBote
                };

                _context.Pruebas.Add(prueba);
                await _context.SaveChangesAsync();

                var resultDto = new PruebaDto
                {
                    IdPrueba = prueba.IdPrueba,
                    Distancia = prueba.Distancia,
                    CategoriaEdad = prueba.CategoriaEdad,
                    SexoCompetencia = prueba.SexoCompetencia,
                    TipoBote = prueba.TipoBote
                };

                var result = new ObjectResult(resultDto)
                {
                    StatusCode = 201
                };
                return result;
            }
            catch (DbUpdateException dbEx)
            {
                return new StatusCodeResult(500);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<IActionResult> PutPrueba(int id, PruebaCreateDto pruebaDto)
        {
            try
            {
                var prueba = await _context.Pruebas.FindAsync(id);

                if (prueba == null)
                {
                    return new NotFoundResult();
                }

                prueba.Distancia = pruebaDto.Distancia;
                prueba.CategoriaEdad = pruebaDto.CategoriaEdad;
                prueba.SexoCompetencia = pruebaDto.SexoCompetencia;
                prueba.TipoBote = pruebaDto.TipoBote;

                await _context.SaveChangesAsync();
                return new NoContentResult();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await PruebaExistsAsync(id))
                {
                    return new NotFoundResult();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<IActionResult> DeletePrueba(int id)
        {
            try
            {
                var prueba = await _context.Pruebas.FindAsync(id);
                if (prueba == null)
                {
                    return new NotFoundResult();
                }

                _context.Pruebas.Remove(prueba);
                await _context.SaveChangesAsync();

                return new NoContentResult();
            }
            catch (DbUpdateException dbEx)
            {
                return new StatusCodeResult(500);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        private async Task<bool> PruebaExistsAsync(int id)
        {
            return await _context.Pruebas.AnyAsync(e => e.IdPrueba == id);
        }
    }
}
