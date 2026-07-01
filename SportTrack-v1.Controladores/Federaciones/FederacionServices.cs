using Microsoft.EntityFrameworkCore;
using SportTrack.AccessDatos;
using SportTrack_v1.Entidades.Entidades;
using SportTrack_v1.Entidades.DTOs.Federacion;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SportTrack_v1.Controladores.Federaciones;
using Microsoft.AspNetCore.Mvc;

namespace SIGDEF.API.Services
{
    public class FederacionServices : IFederacionServices
    {
        private readonly SportTrackDbContext _context;

        public FederacionServices(SportTrackDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult<IEnumerable<FederacionDto>>> GetFederaciones()
        {
            try
            {
                var federaciones = await _context.Federaciones
                    .Select(f => new FederacionDto
                    {
                        IdFederacion = f.IdFederacion,
                        Nombre = f.Nombre,
                        Cuit = f.Cuit,
                        Email = f.Email,
                        Telefono = f.Telefono,
                        Direccion = f.Direccion,
                        BancoNombre = f.BancoNombre,
                        TipoCuenta = f.TipoCuenta,
                        NumeroCuenta = f.NumeroCuenta,
                        TitularCuenta = f.TitularCuenta,
                        EmailCobro = f.EmailCobro
                    })
                    .ToListAsync();

                return new OkObjectResult(federaciones);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<FederacionDto>> GetFederacion(int id)
        {
            try
            {
                var federacion = await _context.Federaciones
                    .Where(f => f.IdFederacion == id)
                    .Select(f => new FederacionDto
                    {
                        IdFederacion = f.IdFederacion,
                        Nombre = f.Nombre,
                        Cuit = f.Cuit,
                        Email = f.Email,
                        Telefono = f.Telefono,
                        Direccion = f.Direccion,
                        BancoNombre = f.BancoNombre,
                        TipoCuenta = f.TipoCuenta,
                        NumeroCuenta = f.NumeroCuenta,
                        TitularCuenta = f.TitularCuenta,
                        EmailCobro = f.EmailCobro
                    })
                    .FirstOrDefaultAsync();

                if (federacion == null)
                {
                    return new NotFoundResult();
                }

                return new OkObjectResult(federacion);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<FederacionDto>> PostFederacion(FederacionCreateDto federacionCreateDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(federacionCreateDto.Nombre) ||
                    string.IsNullOrWhiteSpace(federacionCreateDto.Cuit))
                {
                    return new BadRequestResult();
                }

                var federacion = new Federacion
                {
                    Nombre = federacionCreateDto.Nombre,
                    Cuit = federacionCreateDto.Cuit,
                    Email = federacionCreateDto.Email ?? string.Empty,
                    Telefono = federacionCreateDto.Telefono ?? string.Empty,
                    Direccion = federacionCreateDto.Direccion ?? string.Empty,
                    BancoNombre = federacionCreateDto.BancoNombre ?? string.Empty,
                    TipoCuenta = federacionCreateDto.TipoCuenta ?? string.Empty,
                    NumeroCuenta = federacionCreateDto.NumeroCuenta ?? string.Empty,
                    TitularCuenta = federacionCreateDto.TitularCuenta ?? string.Empty,
                    EmailCobro = federacionCreateDto.EmailCobro ?? string.Empty
                };

                _context.Federaciones.Add(federacion);
                await _context.SaveChangesAsync();

                var federacionDto = new FederacionDto
                {
                    IdFederacion = federacion.IdFederacion,
                    Nombre = federacion.Nombre,
                    Cuit = federacion.Cuit,
                    Email = federacion.Email,
                    Telefono = federacion.Telefono,
                    Direccion = federacion.Direccion,
                    BancoNombre = federacion.BancoNombre,
                    TipoCuenta = federacion.TipoCuenta,
                    NumeroCuenta = federacion.NumeroCuenta,
                    TitularCuenta = federacion.TitularCuenta,
                    EmailCobro = federacion.EmailCobro
                };

                var result = new ObjectResult(federacionDto)
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

        public async Task<IActionResult> PutFederacion(int id, FederacionCreateDto federacionCreateDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(federacionCreateDto.Nombre) ||
                    string.IsNullOrWhiteSpace(federacionCreateDto.Cuit))
                {
                    return new BadRequestResult();
                }

                var federacion = await _context.Federaciones.FindAsync(id);
                if (federacion == null)
                {
                    return new NotFoundResult();
                }

                federacion.Nombre = federacionCreateDto.Nombre;
                federacion.Cuit = federacionCreateDto.Cuit;
                federacion.Email = federacionCreateDto.Email ?? string.Empty;
                federacion.Telefono = federacionCreateDto.Telefono ?? string.Empty;
                federacion.Direccion = federacionCreateDto.Direccion ?? string.Empty;
                federacion.BancoNombre = federacionCreateDto.BancoNombre ?? string.Empty;
                federacion.TipoCuenta = federacionCreateDto.TipoCuenta ?? string.Empty;
                federacion.NumeroCuenta = federacionCreateDto.NumeroCuenta ?? string.Empty;
                federacion.TitularCuenta = federacionCreateDto.TitularCuenta ?? string.Empty;
                federacion.EmailCobro = federacionCreateDto.EmailCobro ?? string.Empty;

                await _context.SaveChangesAsync();
                return new NoContentResult();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await FederacionExistsAsync(id))
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

        public async Task<IActionResult> DeleteFederacion(int id)
        {
            try
            {
                var federacion = await _context.Federaciones.FindAsync(id);
                if (federacion == null)
                {
                    return new NotFoundResult();
                }

                _context.Federaciones.Remove(federacion);
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

        private async Task<bool> FederacionExistsAsync(int id)
        {
            return await _context.Federaciones.AnyAsync(e => e.IdFederacion == id);
        }
    }
}
