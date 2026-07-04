using Microsoft.EntityFrameworkCore;
using SportTrack.AccessDatos;
using SportTrack_v1.Entidades.Entidades;
using SportTrack_v1.Entidades.DTOs.Entrenador;
using SportTrack_v1.Entidades.DTOs.Participante;
using SportTrack_v1.Entidades.DTOs.Club;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SportTrack_v1.Controladores.Federaciones;
using Microsoft.AspNetCore.Mvc;

namespace SIGDEF.API.Services
{
    public class EntrenadorServices : IEntrenadorServices
    {
        private readonly SportTrackDbContext _context;
        private readonly ITenantProvider _tenantProvider;

        public EntrenadorServices(SportTrackDbContext context, ITenantProvider tenantProvider)
        {
            _context = context;
            _tenantProvider = tenantProvider;
        }

        public async Task<ActionResult<IEnumerable<EntrenadorDto>>> GetEntrenadores()
        {
            try
            {
                var query = _context.Entrenadores.AsQueryable();
                var fedId = _tenantProvider.GetFederacionId();
                if (fedId.HasValue)
                {
                    query = query.Where(e => e.IdFederacion == fedId.Value);
                }
                var clubId = _tenantProvider.GetClubId();
                if (clubId.HasValue)
                {
                    query = query.Where(e => e.IdClub == clubId.Value);
                }

                var entrenadores = await query
                    .Include(e => e.Participante)
                    .Include(e => e.Club)
                    .Select(e => new EntrenadorDto
                    {
                        ParticipanteId = e.ParticipanteId,
                        IdClub = e.IdClub ?? 0,
                        PerteneceSeleccion = e.PerteneceSeleccion == true,
                        CategoriaSeleccion = e.CategoriaSeleccion,
                        BecadoEnard = e.BecadoEnard == true,
                        BecadoSdn = e.BecadoSdn == true,
                        MontoBeca = e.MontoBeca ?? 0,
                        PresentoAptoMedico = e.PresentoAptoMedico == true,
                        NombrePersona = e.Participante.Nombre + " " + e.Participante.Apellido,
                        NombreClub = e.Club.Nombre,
                        SiglasClub = e.Club.Siglas
                    })
                    .ToListAsync();

                return new OkObjectResult(entrenadores);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<EntrenadorDetailDto>> GetEntrenador(int id)
        {
            try
            {
                var entrenador = await _context.Entrenadores
                    .Include(e => e.Participante)
                    .Include(e => e.Club)
                    .Where(e => e.ParticipanteId == id)
                    .Select(e => new EntrenadorDetailDto
                    {
                        ParticipanteId = e.ParticipanteId,
                        IdClub = e.IdClub,
                        PerteneceSeleccion = e.PerteneceSeleccion == true,
                        CategoriaSeleccion = e.CategoriaSeleccion,
                        BecadoEnard = e.BecadoEnard == true,
                        BecadoSdn = e.BecadoSdn == true,
                        MontoBeca = e.MontoBeca ?? 0,
                        PresentoAptoMedico = e.PresentoAptoMedico == true,
                        Participante = new PersonaDto
                        {
                            ParticipanteId = e.Participante.ParticipanteId,
                            Nombre = e.Participante.Nombre,
                            Apellido = e.Participante.Apellido,
                            Documento = e.Participante.Dni,
                            FechaNacimiento = e.Participante.FechaNacimiento,
                            Email = e.Participante.Email,
                            Telefono = e.Participante.Telefono,
                            Direccion = e.Participante.Direccion
                        },
                        Club = e.Club != null ? new ClubDto
                        {
                            IdClub = e.Club.IdClub,
                            Nombre = e.Club.Nombre,
                            Direccion = e.Club.Direccion,
                            Telefono = e.Club.Telefono,
                            Siglas = e.Club.Siglas
                        } : null
                    })
                    .FirstOrDefaultAsync();

                if (entrenador == null)
                {
                    return new NotFoundResult();
                }

                return new OkObjectResult(entrenador);
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { error = ex.Message, inner = ex.InnerException?.Message }) { StatusCode = 500 };
            }
        }

        public async Task<ActionResult<IEnumerable<EntrenadorDto>>> GetEntrenadoresPorClub(int idClub)
        {
            try
            {
                var entrenadores = await _context.Entrenadores
                    .Include(e => e.Participante)
                    .Include(e => e.Club)
                    .Where(e => e.IdClub == idClub)
                    .Select(e => new EntrenadorDto
                    {
                        ParticipanteId = e.ParticipanteId,
                        IdClub = e.IdClub ?? 0,
                        PerteneceSeleccion = e.PerteneceSeleccion == true,
                        CategoriaSeleccion = e.CategoriaSeleccion,
                        BecadoEnard = e.BecadoEnard == true,
                        BecadoSdn = e.BecadoSdn == true,
                        MontoBeca = e.MontoBeca ?? 0,
                        PresentoAptoMedico = e.PresentoAptoMedico == true,
                        NombrePersona = e.Participante.Nombre + " " + e.Participante.Apellido,
                        NombreClub = e.Club != null ? e.Club.Nombre : "Agente Libre",
                        SiglasClub = e.Club != null ? e.Club.Siglas : ""
                    })
                    .ToListAsync();

                return new OkObjectResult(entrenadores);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<IEnumerable<EntrenadorDto>>> GetEntrenadoresSeleccion()
        {
            try
            {
                var query = _context.Entrenadores.AsQueryable();
                var fedId = _tenantProvider.GetFederacionId();
                if (fedId.HasValue)
                {
                    query = query.Where(e => e.IdFederacion == fedId.Value);
                }
                var clubId = _tenantProvider.GetClubId();
                if (clubId.HasValue)
                {
                    query = query.Where(e => e.IdClub == clubId.Value);
                }

                var entrenadores = await query
                    .Include(e => e.Participante)
                    .Include(e => e.Club)
                    .Where(e => e.PerteneceSeleccion == true)
                    .Select(e => new EntrenadorDto
                    {
                        ParticipanteId = e.ParticipanteId,
                        IdClub = e.IdClub ?? 0,
                        PerteneceSeleccion = e.PerteneceSeleccion == true,
                        CategoriaSeleccion = e.CategoriaSeleccion,
                        BecadoEnard = e.BecadoEnard == true,
                        BecadoSdn = e.BecadoSdn == true,
                        MontoBeca = e.MontoBeca ?? 0,
                        PresentoAptoMedico = e.PresentoAptoMedico == true,
                        NombrePersona = e.Participante.Nombre + " " + e.Participante.Apellido,
                        NombreClub = e.Club != null ? e.Club.Nombre : "Agente Libre",
                        SiglasClub = e.Club != null ? e.Club.Siglas : ""
                    })
                    .ToListAsync();

                return new OkObjectResult(entrenadores);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<IEnumerable<EntrenadorDto>>> SearchEntrenadores(string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    return new BadRequestResult();
                }

                var query = _context.Entrenadores.AsQueryable();
                var fedId = _tenantProvider.GetFederacionId();
                if (fedId.HasValue)
                {
                    query = query.Where(e => e.IdFederacion == fedId.Value);
                }
                var clubId = _tenantProvider.GetClubId();
                if (clubId.HasValue)
                {
                    query = query.Where(e => e.IdClub == clubId.Value);
                }

                var entrenadores = await query
                    .Include(e => e.Participante)
                    .Include(e => e.Club)
                    .Where(e => e.Participante.Nombre.Contains(term) ||
                                e.Participante.Apellido.Contains(term))
                    .Select(e => new EntrenadorDto
                    {
                        ParticipanteId = e.ParticipanteId,
                        IdClub = e.IdClub ?? 0,
                        PerteneceSeleccion = e.PerteneceSeleccion == true,
                        CategoriaSeleccion = e.CategoriaSeleccion,
                        BecadoEnard = e.BecadoEnard == true,
                        BecadoSdn = e.BecadoSdn == true,
                        MontoBeca = e.MontoBeca ?? 0,
                        PresentoAptoMedico = e.PresentoAptoMedico == true,
                        NombrePersona = e.Participante.Nombre + " " + e.Participante.Apellido,
                        NombreClub = e.Club != null ? e.Club.Nombre : "Agente Libre",
                        SiglasClub = e.Club != null ? e.Club.Siglas : ""
                    })
                    .ToListAsync();

                return new OkObjectResult(entrenadores);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<EntrenadorDto>> PostEntrenador(EntrenadorCreateDto entrenadorCreateDto)
        {
            try
            {
                var personaExists = await _context.Participantes.AnyAsync(p => p.ParticipanteId == entrenadorCreateDto.ParticipanteId);
                if (!personaExists)
                {
                    return new BadRequestResult();
                }

                if (entrenadorCreateDto.IdClub.HasValue && entrenadorCreateDto.IdClub.Value > 0)
                {
                    var clubExists = await _context.Clubes.AnyAsync(c => c.IdClub == entrenadorCreateDto.IdClub.Value);
                    if (!clubExists)
                    {
                        return new BadRequestObjectResult(new { message = "Club no encontrado." });
                    }
                }

                var entrenadorExists = await _context.Entrenadores.AnyAsync(e => e.ParticipanteId == entrenadorCreateDto.ParticipanteId);
                if (entrenadorExists)
                {
                    return new BadRequestResult();
                }

                int? finalFedId = _tenantProvider.GetFederacionId();
                if (!finalFedId.HasValue && entrenadorCreateDto.IdFederacion.HasValue)
                {
                    finalFedId = entrenadorCreateDto.IdFederacion;
                }
                if (!finalFedId.HasValue && entrenadorCreateDto.IdClub.HasValue)
                {
                    var club = await _context.Clubes.FindAsync(entrenadorCreateDto.IdClub.Value);
                    finalFedId = club?.IdFederacion;
                }

                var entrenador = new Entrenador
                {
                    ParticipanteId = entrenadorCreateDto.ParticipanteId,
                    IdClub = entrenadorCreateDto.IdClub,
                    IdFederacion = finalFedId,
                    PerteneceSeleccion = entrenadorCreateDto.PerteneceSeleccion,
                    CategoriaSeleccion = entrenadorCreateDto.CategoriaSeleccion,
                    BecadoEnard = entrenadorCreateDto.BecadoEnard,
                    BecadoSdn = entrenadorCreateDto.BecadoSdn,
                    MontoBeca = entrenadorCreateDto.MontoBeca,
                    PresentoAptoMedico = entrenadorCreateDto.PresentoAptoMedico
                };

                _context.Entrenadores.Add(entrenador);
                await _context.SaveChangesAsync();

                await _context.Entry(entrenador)
                    .Reference(e => e.Participante)
                    .LoadAsync();
                await _context.Entry(entrenador)
                    .Reference(e => e.Club)
                    .LoadAsync();

                var entrenadorDto = new EntrenadorDto
                {
                    ParticipanteId = entrenador.ParticipanteId,
                    IdClub = entrenador.IdClub ?? 0,
                    PerteneceSeleccion = entrenador.PerteneceSeleccion == true,
                    CategoriaSeleccion = entrenador.CategoriaSeleccion,
                    BecadoEnard = entrenador.BecadoEnard == true,
                    BecadoSdn = entrenador.BecadoSdn == true,
                    MontoBeca = entrenador.MontoBeca ?? 0,
                    PresentoAptoMedico = entrenador.PresentoAptoMedico == true,
                    NombrePersona = entrenador.Participante.Nombre + " " + entrenador.Participante.Apellido,
                    NombreClub = entrenador.Club.Nombre,
                    SiglasClub = entrenador.Club.Siglas
                };

                var result = new ObjectResult(entrenadorDto)
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

        public async Task<IActionResult> PutEntrenador(int id, EntrenadorCreateDto entrenadorCreateDto)
        {
            try
            {
                if (id != entrenadorCreateDto.ParticipanteId)
                {
                    return new BadRequestResult();
                }

                var entrenador = await _context.Entrenadores.FindAsync(id);
                if (entrenador == null)
                {
                    return new NotFoundResult();
                }

                if (entrenadorCreateDto.IdClub.HasValue && entrenadorCreateDto.IdClub.Value > 0)
                {
                    var clubExists = await _context.Clubes.AnyAsync(c => c.IdClub == entrenadorCreateDto.IdClub.Value);
                    if (!clubExists)
                    {
                        return new BadRequestObjectResult(new { message = "Club no encontrado." });
                    }
                }

                entrenador.IdClub = entrenadorCreateDto.IdClub;
                entrenador.PerteneceSeleccion = entrenadorCreateDto.PerteneceSeleccion;
                entrenador.CategoriaSeleccion = entrenadorCreateDto.CategoriaSeleccion;
                entrenador.BecadoEnard = entrenadorCreateDto.BecadoEnard;
                entrenador.BecadoSdn = entrenadorCreateDto.BecadoSdn;
                entrenador.MontoBeca = entrenadorCreateDto.MontoBeca;
                entrenador.PresentoAptoMedico = entrenadorCreateDto.PresentoAptoMedico;

                await _context.SaveChangesAsync();

                return new NoContentResult();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EntrenadorExistsAsync(id))
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

        public async Task<IActionResult> DeleteEntrenador(int id)
        {
            try
            {
                var entrenador = await _context.Entrenadores.FindAsync(id);
                if (entrenador == null)
                {
                    return new NotFoundResult();
                }

                _context.Entrenadores.Remove(entrenador);
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

        private async Task<bool> EntrenadorExistsAsync(int id)
        {
            return await _context.Entrenadores.AnyAsync(e => e.ParticipanteId == id);
        }
    }
}
