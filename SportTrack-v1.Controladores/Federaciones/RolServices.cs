using Microsoft.EntityFrameworkCore;
using SportTrack.AccessDatos;
using SportTrack_v1.Controladores.Extensions;
using SportTrack_v1.Entidades.Entidades;
using SportTrack_v1.Entidades.DTOs.Rol;
using SportTrack_v1.Entidades.DTOs.DelegadoClub;
using SportTrack_v1.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SportTrack_v1.Controladores.Federaciones;
using Microsoft.AspNetCore.Mvc;

namespace SIGDEF.API.Services
{
    public class RolServices : IRolServices
    {
        private readonly SportTrackDbContext _context;

        public RolServices(SportTrackDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult<IEnumerable<RolDto>>> GetRoles()
        {
            try
            {
                var roles = await _context.Roles
                    .Select(r => new RolDto
                    {
                        IdRol = r.IdRol,
                        Tipo = r.Tipo,
                        TipoEnum = r.TipoEnum,
                        CantidadRepresentantes = r.DelegadosClub.Count
                    })
                    .ToListAsync();

                return new OkObjectResult(roles);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<RolDetailDto>> GetRol(int id)
        {
            try
            {
                var rol = await _context.Roles
                    .Include(r => r.DelegadosClub)
                        .ThenInclude(d => d.Participante)
                    .Include(r => r.DelegadosClub)
                        .ThenInclude(d => d.Federacion)
                    .Where(r => r.IdRol == id)
                    .Select(r => new RolDetailDto
                    {
                        IdRol = r.IdRol,
                        Tipo = r.Tipo,
                        TipoEnum = r.TipoEnum,
                        Representantes = r.DelegadosClub.Select(d => new DelegadoClubDto
                        {
                            ParticipanteId = d.IdParticipante,
                            IdRol = d.IdRol,
                            IdFederacion = d.IdFederacion,
                            NombrePersona = d.Participante.Nombre + " " + d.Participante.Apellido,
                            NombreFederacion = d.Federacion.Nombre
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (rol == null)
                {
                    return new NotFoundResult();
                }

                return new OkObjectResult(rol);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<IEnumerable<RolDto>>> SearchRoles(string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    return new BadRequestResult();
                }

                var roles = await _context.Roles
                    .Where(r => r.Tipo.Contains(term))
                    .Select(r => new RolDto
                    {
                        IdRol = r.IdRol,
                        Tipo = r.Tipo,
                        TipoEnum = r.TipoEnum,
                        CantidadRepresentantes = r.DelegadosClub.Count
                    })
                    .ToListAsync();

                return new OkObjectResult(roles);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<RolDto>> GetRolPorTipo(string tipo)
        {
            try
            {
                if (Enum.TryParse<RolTipo>(tipo, true, out var rolTipo))
                {
                    var rol = await _context.Roles.GetByTipoAsync(rolTipo);

                    if (rol == null)
                    {
                        return new NotFoundResult();
                    }

                    var rolDto = new RolDto
                    {
                        IdRol = rol.IdRol,
                        Tipo = rol.Tipo,
                        TipoEnum = rol.TipoEnum,
                        CantidadRepresentantes = rol.DelegadosClub.Count
                    };

                    return new OkObjectResult(rolDto);
                }
                else
                {
                    var rol = await _context.Roles
                        .Where(r => r.Tipo.ToLower() == tipo.ToLower())
                        .Select(r => new RolDto
                        {
                            IdRol = r.IdRol,
                            Tipo = r.Tipo,
                            TipoEnum = r.TipoEnum,
                            CantidadRepresentantes = r.DelegadosClub.Count
                        })
                        .FirstOrDefaultAsync();

                    if (rol == null)
                    {
                        return new NotFoundResult();
                    }

                    return new OkObjectResult(rol);
                }
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<IEnumerable<RolDto>>> GetRolesPredefinidos()
        {
            try
            {
                var rolesPredefinidos = new List<RolTipo>
                {
                    RolTipo.Administrador,
                    RolTipo.DelegadoClub,
                    RolTipo.Entrenador,
                    RolTipo.Atleta,
                };

                var roles = await _context.Roles.GetByTiposAsync(rolesPredefinidos.ToArray());

                var rolesDto = roles.Select(r => new RolDto
                {
                    IdRol = r.IdRol,
                    Tipo = r.Tipo,
                    TipoEnum = r.TipoEnum,
                    CantidadRepresentantes = r.DelegadosClub.Count
                }).ToList();

                return new OkObjectResult(rolesDto);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult> GetEnumValues()
        {
            try
            {
                var enumValues = Enum.GetValues(typeof(RolTipo))
                    .Cast<RolTipo>()
                    .Select(e => new
                    {
                        Id = (int)e,
                        Nombre = e.ToString(),
                        Descripcion = GetRoleDescription(e)
                    })
                    .ToList();

                return new OkObjectResult(enumValues);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<RolDto>> GetRolPorEnumId(int enumId)
        {
            try
            {
                if (!Enum.IsDefined(typeof(RolTipo), enumId))
                {
                    return new BadRequestResult();
                }

                var rolTipo = (RolTipo)enumId;
                var rol = await _context.Roles.GetByTipoAsync(rolTipo);

                if (rol == null)
                {
                    return new NotFoundResult();
                }

                var rolDto = new RolDto
                {
                    IdRol = rol.IdRol,
                    Tipo = rol.Tipo,
                    TipoEnum = rol.TipoEnum,
                    CantidadRepresentantes = rol.DelegadosClub.Count
                };

                return new OkObjectResult(rolDto);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<RolDto>> PostRol(RolCreateDto rolCreateDto)
        {
            try
            {
                if (!Enum.TryParse<RolTipo>(rolCreateDto.Tipo, true, out var rolTipo))
                {
                    return new BadRequestResult();
                }

                var rolExistente = await _context.Roles.ExistsByTipoAsync(rolTipo);
                if (rolExistente)
                {
                    return new BadRequestResult();
                }

                var rol = new Rol
                {
                    Tipo = rolCreateDto.Tipo
                };

                _context.Roles.Add(rol);
                await _context.SaveChangesAsync();

                var rolDto = new RolDto
                {
                    IdRol = rol.IdRol,
                    Tipo = rol.Tipo,
                    TipoEnum = rol.TipoEnum,
                    CantidadRepresentantes = 0
                };

                var result = new ObjectResult(rolDto)
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

        public async Task<IActionResult> PutRol(int id, RolCreateDto rolCreateDto)
        {
            try
            {
                if (!Enum.TryParse<RolTipo>(rolCreateDto.Tipo, true, out var rolTipo))
                {
                    return new BadRequestResult();
                }

                var rol = await _context.Roles.FindAsync(id);
                if (rol == null)
                {
                    return new NotFoundResult();
                }

                var otroRolConMismoTipo = await _context.Roles.GetByTipoAsync(rolTipo);
                if (otroRolConMismoTipo != null && otroRolConMismoTipo.IdRol != id)
                {
                    return new BadRequestResult();
                }

                rol.Tipo = rolCreateDto.Tipo;
                await _context.SaveChangesAsync();
                return new NoContentResult();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await RolExistsAsync(id))
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

        public async Task<IActionResult> DeleteRol(int id)
        {
            try
            {
                var rol = await _context.Roles
                    .Include(r => r.DelegadosClub)
                    .FirstOrDefaultAsync(r => r.IdRol == id);

                if (rol == null)
                {
                    return new NotFoundResult();
                }

                if (rol.DelegadosClub.Any())
                {
                    return new BadRequestResult();
                }

                _context.Roles.Remove(rol);
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

        private async Task<bool> RolExistsAsync(int id)
        {
            return await _context.Roles.AnyAsync(e => e.IdRol == id);
        }

        private string GetRoleDescription(RolTipo tipo)
        {
            return tipo switch
            {
                RolTipo.Administrador => "Acceso total al sistema",
                RolTipo.PresidenteFederacion => "MÃ¡xima autoridad de una federaciÃ³n",
                RolTipo.DelegadoClub => "Representante oficial de un club",
                RolTipo.Entrenador => "Entrenador de club",
                RolTipo.EntrenadorSeleccion => "Entrenador de selecciÃ³n nacional",
                RolTipo.Atleta => "Deportista registrado",
                RolTipo.Secretario => "Personal administrativo",
                _ => "Sin descripciÃ³n"
            };
        }
    }
}
