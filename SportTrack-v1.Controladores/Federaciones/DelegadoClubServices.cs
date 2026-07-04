using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportTrack.AccessDatos;
using SportTrack_v1.Entidades.DTOs.DelegadoClub;
using SportTrack_v1.Entidades.DTOs.Participante;
using SportTrack_v1.Entidades.DTOs.Rol;
using SportTrack_v1.Entidades.DTOs.Federacion;
using SportTrack_v1.Entidades.DTOs.Club;
using SportTrack_v1.Entidades.Entidades;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SportTrack_v1.Controladores.Federaciones;

namespace SIGDEF.API.Services
{
    public class DelegadoClubServices : IDelegadoClubServices
    {
        private readonly SportTrackDbContext _context;

        public DelegadoClubServices(SportTrackDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult<IEnumerable<DelegadoClubDto>>> GetDelegadosClub()
        {
            try
            {
                var delegados = await _context.DelegadosClub
                    .Include(d => d.Participante)
                    .Include(d => d.Rol)
                    .Include(d => d.Federacion)
                    .Include(d => d.Club)
                    .Select(d => new DelegadoClubDto
                    {
                        ParticipanteId = d.ParticipanteId,
                        IdRol = d.IdRol,
                        IdFederacion = d.IdFederacion,
                        IdClub = d.ClubIdClub ?? 0,
                        NombrePersona = d.Participante.Nombre + " " + d.Participante.Apellido,
                        TipoRol = d.Rol.Tipo,
                        NombreFederacion = d.Federacion.Nombre,
                        NombreClub = d.Club.Nombre,
                        Documento = d.Participante.Dni,
                        Email = d.Participante.Email,
                        Telefono = d.Participante.Telefono
                    })
                    .ToListAsync();

                return new OkObjectResult(delegados);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<DelegadoClubDetailDto>> GetDelegadoClub(int id)
        {
            try
            {
                var delegado = await _context.DelegadosClub
                    .Include(d => d.Participante)
                    .Include(d => d.Rol)
                    .Include(d => d.Federacion)
                    .Include(d => d.Club)
                    .Where(d => d.ParticipanteId == id)
                    .Select(d => new DelegadoClubDetailDto
                    {
                        ParticipanteId = d.ParticipanteId,
                        IdRol = d.IdRol,
                        IdFederacion = d.IdFederacion,
                        IdClub = d.ClubIdClub ?? null,
                        Participante = new PersonaDto
                        {
                            ParticipanteId = d.Participante.ParticipanteId,
                            Nombre = d.Participante.Nombre,
                            Apellido = d.Participante.Apellido,
                            Documento = d.Participante.Dni,
                            FechaNacimiento = d.Participante.FechaNacimiento,
                            Email = d.Participante.Email,
                            Telefono = d.Participante.Telefono,
                            Direccion = d.Participante.Direccion
                        },
                        Rol = new RolDto
                        {
                            IdRol = d.Rol.IdRol,
                            Tipo = d.Rol.Tipo
                        },
                        Federacion = new FederacionDto
                        {
                            IdFederacion = d.Federacion.IdFederacion,
                            Nombre = d.Federacion.Nombre,
                            Cuit = d.Federacion.Cuit,
                            Email = d.Federacion.Email,
                            Telefono = d.Federacion.Telefono
                        },
                        Club = d.Club != null ? new ClubDto
                        {
                            IdClub = d.Club.IdClub,
                            Nombre = d.Club.Nombre,
                            Direccion = d.Club.Direccion,
                            Telefono = d.Club.Telefono,
                            Siglas = d.Club.Siglas
                        } : null
                    })
                    .FirstOrDefaultAsync();

                if (delegado == null)
                {
                    return new NotFoundResult();
                }

                return new OkObjectResult(delegado);
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { error = ex.Message, inner = ex.InnerException?.Message }) { StatusCode = 500 };
            }
        }

        public async Task<ActionResult<IEnumerable<DelegadoClubDto>>> GetDelegadosPorFederacion(int idFederacion)
        {
            try
            {
                var delegados = await _context.DelegadosClub
                    .Include(d => d.Participante)
                    .Include(d => d.Rol)
                    .Include(d => d.Federacion)
                    .Include(d => d.Club)
                    .Where(d => d.IdFederacion == idFederacion)
                    .Select(d => new DelegadoClubDto
                    {
                        ParticipanteId = d.ParticipanteId,
                        IdRol = d.IdRol,
                        IdFederacion = d.IdFederacion,
                        IdClub = d.ClubIdClub ?? 0,
                        NombrePersona = d.Participante.Nombre + " " + d.Participante.Apellido,
                        TipoRol = d.Rol.Tipo,
                        NombreFederacion = d.Federacion.Nombre,
                        NombreClub = d.Club.Nombre,
                        Documento = d.Participante.Dni,
                        Email = d.Participante.Email,
                        Telefono = d.Participante.Telefono
                    })
                    .ToListAsync();

                return new OkObjectResult(delegados);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<DelegadoClubDto>> PostDelegadoClub(DelegadoClubCreateDto delegadoClubCreateDto)
        {
            try
            {
                var personaExists = await _context.Participantes.AnyAsync(p => p.ParticipanteId == delegadoClubCreateDto.ParticipanteId);
                if (!personaExists)
                {
                    return new BadRequestResult();
                }

                var rolExists = await _context.Roles.AnyAsync(r => r.IdRol == delegadoClubCreateDto.IdRol);
                if (!rolExists)
                {
                    return new BadRequestResult();
                }

                if (delegadoClubCreateDto.IdFederacion.HasValue && delegadoClubCreateDto.IdFederacion.Value > 0)
                {
                    var federacionExists = await _context.Federaciones.AnyAsync(f => f.IdFederacion == delegadoClubCreateDto.IdFederacion.Value);
                    if (!federacionExists)
                    {
                        return new BadRequestObjectResult(new { message = "FederaciÃ³n no encontrada." });
                    }
                }

                if (delegadoClubCreateDto.IdClub.HasValue && delegadoClubCreateDto.IdClub.Value > 0)
                {
                    var clubExists = await _context.Clubes.AnyAsync(c => c.IdClub == delegadoClubCreateDto.IdClub.Value);
                    if (!clubExists)
                    {
                        return new BadRequestObjectResult(new { message = "Club no encontrado." });
                    }
                }

                var delegadoExists = await _context.DelegadosClub.AnyAsync(d => d.ParticipanteId == delegadoClubCreateDto.ParticipanteId);
                if (delegadoExists)
                {
                    return new BadRequestResult();
                }

                var delegadoClub = new DelegadoClub
                {
                    ParticipanteId = delegadoClubCreateDto.ParticipanteId,
                    IdRol = delegadoClubCreateDto.IdRol,
                    IdFederacion = delegadoClubCreateDto.IdFederacion,
                    ClubIdClub = delegadoClubCreateDto.IdClub
                };

                _context.DelegadosClub.Add(delegadoClub);
                await _context.SaveChangesAsync();

                await _context.Entry(delegadoClub)
                    .Reference(d => d.Participante)
                    .LoadAsync();
                await _context.Entry(delegadoClub)
                    .Reference(d => d.Rol)
                    .LoadAsync();
                await _context.Entry(delegadoClub)
                    .Reference(d => d.Federacion)
                    .LoadAsync();
                await _context.Entry(delegadoClub)
                    .Reference(d => d.Club)
                    .LoadAsync();

                var delegadoClubDto = new DelegadoClubDto
                {
                    ParticipanteId = delegadoClub.ParticipanteId,
                    IdRol = delegadoClub.IdRol,
                    IdFederacion = delegadoClub.IdFederacion,
                    IdClub = delegadoClub.ClubIdClub ?? 0,
                    NombrePersona = delegadoClub.Participante.Nombre + " " + delegadoClub.Participante.Apellido,
                    TipoRol = delegadoClub.Rol?.Tipo ?? "Delegado",
                    NombreFederacion = delegadoClub.Federacion?.Nombre ?? "N/A",
                    NombreClub = delegadoClub.Club?.Nombre ?? "Agente Libre"
                };

                var result = new ObjectResult(delegadoClubDto)
                {
                    StatusCode = 201
                };
                return result;
            }
            catch (DbUpdateException dbEx)
            {
                return new ObjectResult(new { error = "Error de base de datos", detail = dbEx.Message, inner = dbEx.InnerException?.Message }) { StatusCode = 500 };
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { error = ex.Message, inner = ex.InnerException?.Message }) { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> PutDelegadoClub(int id, DelegadoClubCreateDto delegadoClubCreateDto)
        {
            try
            {
                if (id != delegadoClubCreateDto.ParticipanteId)
                {
                    return new BadRequestResult();
                }

                var delegadoClub = await _context.DelegadosClub.FindAsync(id);
                if (delegadoClub == null)
                {
                    return new NotFoundResult();
                }

                var rolExists = await _context.Roles.AnyAsync(r => r.IdRol == delegadoClubCreateDto.IdRol);
                if (!rolExists)
                {
                    return new BadRequestResult();
                }

                if (delegadoClubCreateDto.IdFederacion.HasValue && delegadoClubCreateDto.IdFederacion.Value > 0)
                {
                    var federacionExists = await _context.Federaciones.AnyAsync(f => f.IdFederacion == delegadoClubCreateDto.IdFederacion.Value);
                    if (!federacionExists)
                    {
                        return new BadRequestObjectResult(new { message = "FederaciÃ³n no encontrada." });
                    }
                }

                if (delegadoClubCreateDto.IdClub.HasValue && delegadoClubCreateDto.IdClub.Value > 0)
                {
                    var clubExists = await _context.Clubes.AnyAsync(c => c.IdClub == delegadoClubCreateDto.IdClub.Value);
                    if (!clubExists)
                    {
                        return new BadRequestObjectResult(new { message = "Club no encontrado." });
                    }
                }

                delegadoClub.IdRol = delegadoClubCreateDto.IdRol;
                delegadoClub.IdFederacion = delegadoClubCreateDto.IdFederacion;
                delegadoClub.ClubIdClub = delegadoClubCreateDto.IdClub;

                await _context.SaveChangesAsync();

                return new NoContentResult();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await DelegadoClubExistsAsync(id))
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
                return new ObjectResult(new { error = ex.Message, inner = ex.InnerException?.Message }) { StatusCode = 500 };
            }
        }

        public async Task<IActionResult> DeleteDelegadoClub(int id)
        {
            try
            {
                var delegadoClub = await _context.DelegadosClub.FindAsync(id);
                if (delegadoClub == null)
                {
                    return new NotFoundResult();
                }

                _context.DelegadosClub.Remove(delegadoClub);
                await _context.SaveChangesAsync();

                return new NoContentResult();
            }
            catch (DbUpdateException)
            {
                return new StatusCodeResult(500);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        private async Task<bool> DelegadoClubExistsAsync(int id)
        {
            return await _context.DelegadosClub.AnyAsync(e => e.ParticipanteId == id);
        }
    }
}
