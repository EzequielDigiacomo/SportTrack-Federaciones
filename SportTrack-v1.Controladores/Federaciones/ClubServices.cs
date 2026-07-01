using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportTrack.AccessDatos;
using SportTrack_v1.Entidades.Entidades;
using SportTrack_v1.Entidades.DTOs.Club;
using SportTrack_v1.Entidades.DTOs.AtletaFederado;
using SportTrack_v1.Entidades.DTOs.Entrenador;
using SportTrack_v1.Entidades.DTOs.DelegadoClub;
using SportTrack_v1.Entidades.DTOs.PagoTransaccion;
using SportTrack_v1.Entidades.DTOs.Evento;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SportTrack_v1.Controladores.Federaciones;

namespace SIGDEF.API.Services
{
    public class ClubServices : IClubServices
    {
        private readonly SportTrackDbContext _context;
        private readonly ITenantProvider _tenantProvider;

        public ClubServices(SportTrackDbContext context, ITenantProvider tenantProvider)
        {
            _context = context;
            _tenantProvider = tenantProvider;
        }

        public async Task<ActionResult<IEnumerable<ClubDto>>> GetClubes()
        {
            try
            {
                var query = _context.Clubes.AsQueryable();
                var fedId = _tenantProvider.GetFederacionId();
                if (fedId.HasValue)
                {
                    query = query.Where(c => c.IdFederacion == fedId.Value);
                }

                var clubes = await query
                    .Include(c => c.AtletasFederados)
                    .Include(c => c.Entrenadores)
                    .Include(c => c.Representantes)
                    .ToListAsync();

                var clubesDto = clubes.Select(c => new ClubDto
                {
                    IdClub = c.IdClub,
                    Nombre = c.Nombre,
                    Direccion = c.Direccion,
                    Telefono = c.Telefono,
                    Siglas = c.Siglas,
                    IdFederacion = c.IdFederacion,
                    EstadoMatricula = c.EstadoMatricula,
                    CantidadAtletas = c.AtletasFederados.Count,
                    CantidadEntrenadores = c.Entrenadores.Count,
                    TieneDelegado = c.Representantes.Any(),
                    AtletasPorCategoria = c.AtletasFederados
                        .Where(a => a.Categoria != null)
                        .GroupBy(a => a.Categoria)
                        .ToDictionary(g => (int)g.Key!, g => g.Count())
                }).ToList();

                return new OkObjectResult(clubesDto);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<ClubDetailDto>> GetClub(int id)
        {
            try
            {
                var club = await _context.Clubes
                    .Include(c => c.AtletasFederados).ThenInclude(a => a.Participante)
                    .Include(c => c.Entrenadores).ThenInclude(e => e.Participante)
                    .Include(c => c.Representantes).ThenInclude(r => r.Participante)
                    .Include(c => c.Pagos)
                    .Where(c => c.IdClub == id)
                    .Select(c => new ClubDetailDto
                    {
                        IdClub = c.IdClub,
                        Nombre = c.Nombre,
                        Direccion = c.Direccion,
                        Telefono = c.Telefono,
                        Siglas = c.Siglas,
                        IdFederacion = c.IdFederacion,
                        EstadoMatricula = c.EstadoMatricula,
                        AtletasFederados = c.AtletasFederados.Select(a => new AtletaDto
                        {
                            ParticipanteId = a.ParticipanteId,
                            IdClub = a.IdClub,
                            EstadoPago = a.EstadoPago,
                            PerteneceSeleccion = a.PerteneceSeleccion,
                            Categoria = a.Categoria,
                            BecadoEnard = a.BecadoEnard,
                            BecadoSdn = a.BecadoSdn,
                            MontoBeca = a.MontoBeca,
                            PresentoAptoMedico = a.PresentoAptoMedico,
                            FechaAptoMedico = a.FechaAptoMedico,
                            NombrePersona = a.Participante.Nombre + " " + a.Participante.Apellido
                        }).ToList(),
                        Entrenadores = c.Entrenadores.Select(e => new EntrenadorDto
                        {
                            ParticipanteId = e.ParticipanteId,
                            IdClub = e.IdClub ?? 0,
                            PerteneceSeleccion = e.PerteneceSeleccion == null,
                            CategoriaSeleccion = e.CategoriaSeleccion,
                            BecadoEnard = e.BecadoEnard == null,
                            BecadoSdn = e.BecadoSdn == null,
                            MontoBeca = e.MontoBeca ?? 0,
                            PresentoAptoMedico = e.PresentoAptoMedico== null,
                            NombrePersona = e.Participante.Nombre + " " + e.Participante.Apellido
                        }).ToList(),
                        Representantes = c.Representantes.Select(r => new DelegadoClubDto
                        {
                            ParticipanteId = r.ParticipanteId,
                            IdRol = r.IdRol,
                            IdFederacion = r.IdFederacion,
                            NombrePersona = r.Participante.Nombre + " " + r.Participante.Apellido
                        }).ToList(),
                        Pagos = c.Pagos.Select(p => new PagoTransaccionDto
                        {
                            IdPago = p.IdPago,
                            Concepto = p.Concepto,
                            Monto = p.Monto,
                            Estado = p.Estado,
                            FechaCreacion = p.FechaCreacion,
                            FechaAprobacion = p.FechaAprobacion,
                            ParticipanteId = p.ParticipanteId,
                            IdClub = p.IdClub,
                            IdMercadoPago = p.IdMercadoPago
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (club == null)
                {
                    return new NotFoundResult();
                }

                return new OkObjectResult(club);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<IEnumerable<AtletaDto>>> GetAtletasByClub(int id)
        {
            try
            {
                var AtletasFederados = await _context.AtletasFederados
                    .Include(a => a.Participante)
                    .Where(a => a.IdClub == id)
                    .Select(a => new AtletaDto
                    {
                        ParticipanteId = a.ParticipanteId,
                        IdClub = a.IdClub,
                        EstadoPago = a.EstadoPago,
                        PerteneceSeleccion = a.PerteneceSeleccion,
                        Categoria = a.Categoria,
                        BecadoEnard = a.BecadoEnard,
                        BecadoSdn = a.BecadoSdn,
                        MontoBeca = a.MontoBeca,
                        PresentoAptoMedico = a.PresentoAptoMedico,
                        FechaAptoMedico = a.FechaAptoMedico,
                        NombrePersona = a.Participante.Nombre + " " + a.Participante.Apellido,
                    })
                    .ToListAsync();

                return new OkObjectResult(AtletasFederados);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<IEnumerable<EntrenadorDto>>> GetEntrenadoresByClub(int id)
        {
            try
            {
                var entrenadores = await _context.Entrenadores
                    .Include(e => e.Participante)
                    .Where(e => e.IdClub == id)
                    .Select(e => new EntrenadorDto
                    {
                        ParticipanteId = e.ParticipanteId,
                        IdClub = e.IdClub ?? 0,
                        PerteneceSeleccion = e.PerteneceSeleccion == null,
                        CategoriaSeleccion = e.CategoriaSeleccion,
                        BecadoEnard = e.BecadoEnard == null,
                        BecadoSdn = e.BecadoSdn == null,
                        MontoBeca = e.MontoBeca ?? 0,
                        PresentoAptoMedico = e.PresentoAptoMedico == null,
                        NombrePersona = e.Participante.Nombre + " " + e.Participante.Apellido
                    })
                    .ToListAsync();

                return new OkObjectResult(entrenadores);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<IEnumerable<DelegadoClubDto>>> GetDelegadosByClub(int id)
        {
            try
            {
                var delegados = await _context.DelegadosClub
                    .Include(d => d.Participante)
                    .Include(d => d.Club)
                    .Where(d => d.Club.IdClub == id)
                    .Select(r => new DelegadoClubDto
                    {
                        ParticipanteId = r.ParticipanteId,
                        IdRol = r.IdRol,
                        IdFederacion = r.IdFederacion,
                        IdClub = r.Club.IdClub,
                        NombrePersona = r.Participante.Nombre + " " + r.Participante.Apellido,
                        Documento = r.Participante.Dni,
                        Email = r.Participante.Email,
                        Telefono = r.Participante.Telefono,
                        NombreClub = r.Club.Nombre,
                        NombreFederacion = ""
                    })
                    .ToListAsync();

                return new OkObjectResult(delegados);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<IEnumerable<EventoDto>>> GetEventosByClub(int id)
        {
            try
            {
                var eventos = await _context.Eventos
                    .Include(e => e.Inscripciones)
                        .ThenInclude(i => i.AtletaFederado)
                    .Where(e => e.IdClub == id)
                    .Select(e => new EventoDto
                    {
                        IdEvento = e.IdEvento,
                        Nombre = e.Nombre ?? string.Empty,
                        Descripcion = e.Descripcion ?? string.Empty,
                        TipoEvento = e.TipoEvento.ToString(),
                        FechaInicio = e.FechaInicio,
                        FechaFin = e.FechaFin,
                        Ubicacion = e.Ubicacion ?? string.Empty,
                        IdClub = e.IdClub ?? 0,
                        Estado = e.EstaActivo ? "Activo" : "Inactivo",
                        CantidadInscripciones = e.Inscripciones.Count,
                        TotalAtletas = e.Inscripciones.Select(i => i.ParticipanteId).Distinct().Count(),
                        TotalClubes = e.Inscripciones
                                       .Select(i => i.AtletaFederado.IdClub)
                                       .Distinct()
                                       .Count()
                    })
                    .ToListAsync();

                return new OkObjectResult(eventos);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<ClubDto>> PostClub(ClubCreateDto clubCreateDto)
        {
            try
            {
                var siglasExists = await _context.Clubes.AnyAsync(c => c.Siglas == clubCreateDto.Siglas);
                if (siglasExists)
                {
                    return new BadRequestResult();
                }

                var nombreExists = await _context.Clubes.AnyAsync(c => c.Nombre == clubCreateDto.Nombre);
                if (nombreExists)
                {
                    return new BadRequestResult();
                }

                var fedId = _tenantProvider.GetFederacionId();
                var idFederacion = fedId ?? clubCreateDto.IdFederacion;

                var club = new Club
                {
                    Nombre = clubCreateDto.Nombre,
                    Direccion = clubCreateDto.Direccion ?? string.Empty,
                    Telefono = clubCreateDto.Telefono ?? string.Empty,
                    Siglas = clubCreateDto.Siglas,
                    IdFederacion = idFederacion,
                    EstadoMatricula = clubCreateDto.EstadoMatricula
                };

                _context.Clubes.Add(club);
                await _context.SaveChangesAsync();

                var clubDto = new ClubDto
                {
                    IdClub = club.IdClub,
                    Nombre = club.Nombre,
                    Direccion = club.Direccion,
                    Telefono = club.Telefono,
                    Siglas = club.Siglas,
                    IdFederacion = club.IdFederacion,
                    EstadoMatricula = club.EstadoMatricula,
                    CantidadAtletas = 0,
                    CantidadEntrenadores = 0,
                    TieneDelegado = false
                };

                var result = new ObjectResult(clubDto)
                {
                    StatusCode = 201
                };
                return result;
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<IActionResult> PutClub(int id, ClubCreateDto clubCreateDto)
        {
            try
            {
                var club = await _context.Clubes.FindAsync(id);
                if (club == null)
                {
                    return new NotFoundResult();
                }

                var siglasExists = await _context.Clubes.AnyAsync(c => c.Siglas == clubCreateDto.Siglas && c.IdClub != id);
                if (siglasExists)
                {
                    return new BadRequestResult();
                }

                var nombreExists = await _context.Clubes.AnyAsync(c => c.Nombre == clubCreateDto.Nombre && c.IdClub != id);
                if (nombreExists)
                {
                    return new BadRequestResult();
                }

                club.Nombre = clubCreateDto.Nombre;
                club.Direccion = clubCreateDto.Direccion ?? string.Empty;
                club.Telefono = clubCreateDto.Telefono ?? string.Empty;
                club.Siglas = clubCreateDto.Siglas;
                club.IdFederacion = clubCreateDto.IdFederacion;
                club.EstadoMatricula = clubCreateDto.EstadoMatricula;

                await _context.SaveChangesAsync();
                return new NoContentResult();
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<IActionResult> DeleteClub(int id)
        {
            try
            {
                var club = await _context.Clubes
                    .Include(c => c.AtletasFederados)
                    .Include(c => c.Entrenadores)
                    .Include(c => c.Representantes)
                    .Include(c => c.Pagos)
                    .FirstOrDefaultAsync(c => c.IdClub == id);

                if (club == null)
                {
                    return new NotFoundResult();
                }

                if (club.AtletasFederados.Any() || club.Entrenadores.Any() ||
                    club.Representantes.Any() || club.Pagos.Any())
                {
                    return new BadRequestResult();
                }

                _context.Clubes.Remove(club);
                await _context.SaveChangesAsync();
                return new NoContentResult();
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<IEnumerable<ClubDto>>> SearchClubes(string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    return new BadRequestResult();
                }

                var query = _context.Clubes.AsQueryable();
                var fedId = _tenantProvider.GetFederacionId();
                if (fedId.HasValue)
                {
                    query = query.Where(c => c.IdFederacion == fedId.Value);
                }

                var clubes = await query
                    .Where(c => c.Nombre.Contains(term) || c.Siglas.Contains(term))
                    .Select(c => new ClubDto
                    {
                        IdClub = c.IdClub,
                        Nombre = c.Nombre,
                        Direccion = c.Direccion,
                        Telefono = c.Telefono,
                        Siglas = c.Siglas,
                        IdFederacion = c.IdFederacion,
                        EstadoMatricula = c.EstadoMatricula,
                        CantidadAtletas = c.AtletasFederados.Count,
                        CantidadEntrenadores = c.Entrenadores.Count,
                        TieneDelegado = c.Representantes.Any()
                    })
                    .ToListAsync();

                return new OkObjectResult(clubes);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        private async Task<bool> ClubExistsAsync(int id)
        {
            return await _context.Clubes.AnyAsync(e => e.IdClub == id);
        }
    }
}
