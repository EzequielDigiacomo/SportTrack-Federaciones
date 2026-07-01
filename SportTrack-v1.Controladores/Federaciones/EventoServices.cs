using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportTrack.AccessDatos;
using SportTrack_v1.Controladores.Federaciones;
using SIGDEF.DTOs;
using SportTrack_v1.Entidades.Entidades;
using SportTrack_v1.Entidades.DTOs;
using SportTrack_v1.Entidades.DTOs.Evento;
using SportTrack_v1.Entidades.DTOs.EventoPrueba;
using SportTrack_v1.Entidades.Entidades;
using SportTrack_v1.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGDEF.API.Services
{
    public class EventoServices : IEventoServices
    {
        private readonly SportTrackDbContext _context;
        private readonly ITenantProvider _tenantProvider;

        public EventoServices(SportTrackDbContext context, ITenantProvider tenantProvider)
        {
            _context = context;
            _tenantProvider = tenantProvider;
        }

        public async Task<ActionResult<IEnumerable<EventoDto>>> GetEventos(
            bool? activos = null,
            string? tipo = null,
            string? provincia = null,
            int? distancia = null)
        {
            try
            {
                var query = _context.Eventos.AsQueryable();
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

                var result = await query
                    .Include(e => e.Pruebas)
                        .ThenInclude(ep => ep.Prueba)
                    .Where(e =>
                        (!activos.HasValue || e.EstaActivo == activos.Value) &&
                        (string.IsNullOrEmpty(tipo) || e.TipoEvento.ToString() == tipo) &&
                        (string.IsNullOrEmpty(provincia) || e.Provincia == provincia) &&
                        (!distancia.HasValue || e.Pruebas.Any(ed => ed.Prueba.Distancia == (DistanciaRegata)distancia.Value))
                    )
                    .Select(e => new EventoDto
                    {
                        IdEvento = e.IdEvento,
                        Nombre = e.Nombre,
                        FechaInicio = e.FechaInicio,
                        FechaFin = e.FechaFin,
                        CantidadInscripciones = e.Inscripciones.Count,
                        TotalAtletas = e.Inscripciones.Select(i => i.ParticipanteId).Distinct().Count(),
                        TotalClubes = e.Inscripciones
                            .Select(i => i.AtletaFederado.IdClub)
                            .Distinct()
                            .Count(),
                        Estado = e.EstaActivo
                            ? (e.FechaInicio > DateTime.UtcNow ? "Programado"
                               : e.FechaFin < DateTime.UtcNow ? "Finalizado" : "En curso")
                            : "Inactivo"
                    })
                    .OrderBy(e => e.FechaInicio)
                    .ToListAsync();

                return new OkObjectResult(result);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<EventoResponseDto>> GetEvento(int id)
        {
            try
            {
                var evento = await _context.Eventos
                    .Include(e => e.Pruebas)
                        .ThenInclude(ep => ep.Prueba)
                    .Include(e => e.Inscripciones)
                        .ThenInclude(i => i.AtletaFederado)
                    .Include(e => e.Inscripciones)
                    .FirstOrDefaultAsync(e => e.IdEvento == id);

                if (evento == null)
                {
                    return new NotFoundResult();
                }

                return new OkObjectResult(EventoResponseDto.FromEntity(evento));
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<EventoDetailDto>> GetEventoDetalle(int id)
        {
            try
            {
                var evento = await _context.Eventos
                    .Include(e => e.Pruebas)
                        .ThenInclude(ep => ep.Prueba)
                    .Include(e => e.Inscripciones)
                        .ThenInclude(i => i.AtletaFederado)
                    .Include(e => e.Inscripciones)
                    .FirstOrDefaultAsync(e => e.IdEvento == id);

                if (evento == null)
                {
                    return new NotFoundResult();
                }

                var dto = new EventoDetailDto
                {
                    IdEvento = evento.IdEvento,
                    Nombre = evento.Nombre,
                    FechaInicio = evento.FechaInicio,
                    FechaFin = evento.FechaFin,
                };

                return new OkObjectResult(dto);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<EventoResponseDto>> PostEvento(EventoCreateDTO eventoDto)
        {
            try
            {
                if (eventoDto.FechaInicio >= eventoDto.FechaFin)
                {
                    return new BadRequestResult();
                }

                if (eventoDto.FechaInicioInscripciones.HasValue && eventoDto.FechaFinInscripciones.HasValue)
                {
                    if (eventoDto.FechaInicioInscripciones >= eventoDto.FechaFinInscripciones)
                    {
                        return new BadRequestResult();
                    }

                    if (eventoDto.FechaFinInscripciones > eventoDto.FechaInicio)
                    {
                        return new BadRequestResult();
                    }
                }

                if (eventoDto.Distancias == null || !eventoDto.Distancias.Any())
                {
                    return new BadRequestResult();
                }

                var evento = new Evento
                {
                    Nombre = eventoDto.Nombre,
                    Descripcion = eventoDto.Descripcion,
                    TipoEvento = eventoDto.TipoEvento,
                    FechaInicio = eventoDto.FechaInicio,
                    FechaFin = eventoDto.FechaFin,
                    FechaInicioInscripciones = eventoDto.FechaInicioInscripciones,
                    FechaFinInscripciones = eventoDto.FechaFinInscripciones,
                    Ubicacion = eventoDto.Ubicacion,
                    Ciudad = eventoDto.Ciudad,
                    Provincia = eventoDto.Provincia,
                    PrecioBase = eventoDto.PrecioBase,
                    CupoMaximo = eventoDto.CupoMaximo,
                    TieneCronometraje = eventoDto.TieneCronometraje,
                    RequiereCertificadoMedico = eventoDto.RequiereCertificadoMedico,
                    Observaciones = eventoDto.Observaciones,
                    FechaCreacion = DateTime.UtcNow,
                    EstaActivo = true,
                    IdFederacion = _tenantProvider.GetFederacionId(),
                    IdClub = _tenantProvider.GetClubId()
                };

                foreach (var d in eventoDto.Distancias)
                {
                    var idPrueba = await GetOrCreatePruebaId(
                        d.DistanciaRegata,
                        (CategoriaEdad)d.CategoriaEdad,
                        (SexoCompetencia)d.SexoCompetencia,
                        (TipoBote)d.TipoBote);

                    evento.Pruebas.Add(new EventoPrueba
                    {
                        IdPrueba = idPrueba
                    });
                }

                _context.Eventos.Add(evento);
                await _context.SaveChangesAsync();

                await _context.Entry(evento)
                    .Collection(e => e.Pruebas)
                    .Query()
                    .Include(ep => ep.Prueba)
                    .LoadAsync();

                evento.Inscripciones = new List<Inscripcion>();

                var result = new ObjectResult(EventoResponseDto.FromEntity(evento))
                {
                    StatusCode = 201
                };
                return result;
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

        public async Task<IActionResult> PutEvento(int id, EventoUpdateDto eventoDto)
        {
            try
            {
                if (id != eventoDto.IdEvento)
                {
                    return new BadRequestResult();
                }

                var evento = await _context.Eventos
                    .Include(e => e.Pruebas)
                        .ThenInclude(ep => ep.Prueba)
                    .FirstOrDefaultAsync(e => e.IdEvento == id);

                if (evento == null)
                {
                    return new NotFoundResult();
                }

                if (eventoDto.FechaInicio >= eventoDto.FechaFin)
                {
                    return new BadRequestResult();
                }

                evento.Nombre = eventoDto.Nombre;
                evento.Descripcion = eventoDto.Descripcion;
                evento.TipoEvento = eventoDto.TipoEvento;
                evento.FechaInicio = eventoDto.FechaInicio;
                evento.FechaFin = eventoDto.FechaFin;
                evento.FechaInicioInscripciones = eventoDto.FechaInicioInscripciones;
                evento.FechaFinInscripciones = eventoDto.FechaFinInscripciones;
                evento.Ubicacion = eventoDto.Ubicacion;
                evento.Ciudad = eventoDto.Ciudad;
                evento.Provincia = eventoDto.Provincia;
                evento.PrecioBase = eventoDto.PrecioBase;
                evento.CupoMaximo = eventoDto.CupoMaximo;
                evento.TieneCronometraje = eventoDto.TieneCronometraje;
                evento.RequiereCertificadoMedico = eventoDto.RequiereCertificadoMedico;
                evento.Observaciones = eventoDto.Observaciones;
                evento.EstaActivo = eventoDto.EstaActivo;
                evento.FechaActualizacion = DateTime.UtcNow;

                if (eventoDto.Distancias != null && eventoDto.Distancias.Any())
                {
                    _context.EventoPruebas.RemoveRange(evento.Pruebas);
                    evento.Pruebas.Clear();

                    foreach (var d in eventoDto.Distancias)
                    {
                        var idPrueba = await GetOrCreatePruebaId(
                            d.DistanciaRegata,
                            (CategoriaEdad)d.CategoriaEdad,
                            (SexoCompetencia)d.SexoCompetencia,
                            (TipoBote)d.TipoBote);

                        evento.Pruebas.Add(new EventoPrueba
                        {
                            IdPrueba = idPrueba
                        });
                    }
                }

                await _context.SaveChangesAsync();
                return new NoContentResult();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EventoExistsAsync(id))
                {
                    return new NotFoundResult();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<IActionResult> ActivarEvento(int id)
        {
            try
            {
                var evento = await _context.Eventos.FindAsync(id);
                if (evento == null)
                {
                    return new NotFoundResult();
                }

                evento.EstaActivo = true;
                evento.FechaActualizacion = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return new NoContentResult();
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<IActionResult> DesactivarEvento(int id)
        {
            try
            {
                var evento = await _context.Eventos.FindAsync(id);
                if (evento == null)
                {
                    return new NotFoundResult();
                }

                evento.EstaActivo = false;
                evento.FechaActualizacion = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return new NoContentResult();
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<IActionResult> DeleteEvento(int id)
        {
            try
            {
                var evento = await _context.Eventos
                    .Include(e => e.Inscripciones)
                    .Include(e => e.Pruebas)
                        .ThenInclude(ep => ep.Prueba)
                    .FirstOrDefaultAsync(e => e.IdEvento == id);

                if (evento == null)
                {
                    return new NotFoundResult();
                }

                if (evento.Inscripciones.Any())
                {
                    return new BadRequestResult();
                }

                _context.Eventos.Remove(evento);
                await _context.SaveChangesAsync();

                return new NoContentResult();
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<IEnumerable<EventoDto>>> GetProximosEventos()
        {
            try
            {
                var hoy = DateTime.UtcNow;

                var query = _context.Eventos.AsQueryable();
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

                var eventos = await query
                    .Include(e => e.Pruebas)
                        .ThenInclude(ep => ep.Prueba)
                    .Where(e => e.EstaActivo && e.FechaInicio > hoy)
                    .OrderBy(e => e.FechaInicio)
                    .Take(10)
                    .ToListAsync();

                var eventoIds = eventos.Select(e => e.IdEvento).ToList();

                var estadisticas = await _context.Inscripciones
                    .Where(i => eventoIds.Contains(i.IdEventoPrueba))
                    .Include(i => i.AtletaFederado)
                    .Select(i => new
                    {
                        i.IdEventoPrueba,
                        i.ParticipanteId,
                        ClubId = i.AtletaFederado.IdClub
                    })
                    .ToListAsync();

                var statsPorEvento = estadisticas
                    .GroupBy(i => i.IdEventoPrueba)
                    .Select(g => new
                    {
                        EventoId = g.Key,
                        TotalAtletas = g.Select(i => i.ParticipanteId).Distinct().Count(),
                        TotalClubes = g.Select(i => i.ClubId).Distinct().Count(),
                        CantidadInscripciones = g.Count()
                    })
                    .ToDictionary(x => x.EventoId);

                var result = eventos.Select(e =>
                {
                    var stats = statsPorEvento.GetValueOrDefault(e.IdEvento);

                    return new EventoDto
                    {
                        IdEvento = e.IdEvento,
                        Nombre = e.Nombre,
                        FechaInicio = e.FechaInicio,
                        FechaFin = e.FechaFin,
                        CantidadInscripciones = stats?.CantidadInscripciones ?? 0,
                        TotalAtletas = stats?.TotalAtletas ?? 0,
                        TotalClubes = stats?.TotalClubes ?? 0,
                        Estado = "Programado"
                    };
                }).ToList();

                return new OkObjectResult(result);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<IEnumerable<EventoResponseDto>>> GetEventosConInscripcionesAbiertas()
        {
            try
            {
                var query = _context.Eventos.AsQueryable();
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

                var eventos = await query
                    .Include(e => e.Pruebas)
                        .ThenInclude(ep => ep.Prueba)
                    .Include(e => e.Inscripciones)
                    .Where(e => e.EstaActivo && e.PuedeInscribirse())
                    .OrderBy(e => e.FechaInicio)
                    .ToListAsync();

                var result = eventos.Select(EventoResponseDto.FromEntity).ToList();
                return new OkObjectResult(result);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<IEnumerable<EventoResponseDto>>> GetEventosPorDistancia(int distancia)
        {
            try
            {
                var distanciaEnum = (DistanciaRegata)distancia;

                var eventos = await _context.Eventos
                    .Include(e => e.Pruebas)
                        .ThenInclude(ep => ep.Prueba)
                    .Include(e => e.Inscripciones)
                    .Where(e => e.EstaActivo && e.Pruebas.Any(ed => ed.Prueba.Distancia == distanciaEnum))
                    .OrderBy(e => e.FechaInicio)
                    .ToListAsync();

                var result = eventos.Select(EventoResponseDto.FromEntity).ToList();
                return new OkObjectResult(result);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<EventoFormConfigDto>> GetFormConfig()
        {
            try
            {
                var config = new EventoFormConfigDto
                {
                    DistanciasDisponibles = Enum.GetValues(typeof(DistanciaRegata))
                        .Cast<DistanciaRegata>()
                        .Select(d => new DistanciaOptionDto
                        {
                            IdDistanciaEnum = (int)d,
                            CodigoDistanca = GetDistanciaCodigo(d),
                            NombreDistancias = d.ToString(),
                            Metros = GetDistanciaEnMetros(d),
                            TipoDistancia = GetTipoDistancia(d)
                        }).ToList(),

                    CategoriasDisponibles = Enum.GetValues(typeof(CategoriaEdad))
                        .Cast<CategoriaEdad>()
                        .Select(CategoriaOptionDto.FromEnum)
                        .ToList(),

                    TiposEvento = Enum.GetValues(typeof(TipoEvento))
                        .Cast<TipoEvento>()
                        .Select(TipoEventoOptionDto.FromEnum)
                        .ToList(),

                    TiposBote = Enum.GetValues(typeof(TipoBote))
                        .Cast<TipoBote>()
                        .Select(TipoBoteOptionDto.FromEnum)
                        .ToList()
                };

                return new OkObjectResult(config);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        private async Task<bool> EventoExistsAsync(int id)
        {
            return await _context.Eventos.AnyAsync(e => e.IdEvento == id);
        }

        private async Task<int> GetOrCreatePruebaId(DistanciaRegata distancia, CategoriaEdad categoria, SexoCompetencia sexo, TipoBote bote)
        {
            var prueba = await _context.Pruebas.FirstOrDefaultAsync(p =>
                p.Distancia == distancia &&
                p.CategoriaEdad == categoria &&
                p.SexoCompetencia == sexo &&
                p.TipoBote == bote);

            if (prueba != null) return prueba.IdPrueba;

            prueba = new Prueba
            {
                Distancia = distancia,
                CategoriaEdad = categoria,
                SexoCompetencia = sexo,
                TipoBote = bote
            };
            _context.Pruebas.Add(prueba);
            await _context.SaveChangesAsync();
            return prueba.IdPrueba;
        }

        private string GetDistanciaCodigo(DistanciaRegata distancia)
        {
            return distancia switch
            {
                DistanciaRegata.DoscientosMetros => "200m",
                DistanciaRegata.TrecientosCincuentaMetros => "350m",
                DistanciaRegata.QuatroCientosMetros => "400m",
                DistanciaRegata.QuinientosMetros => "500m",
                DistanciaRegata.MilMetros => "1000m",
                DistanciaRegata.DosKilometros => "2K",
                DistanciaRegata.TresKilometros => "3K",
                DistanciaRegata.CincoKilometros => "5K",
                DistanciaRegata.DiezKilometros => "10K",
                DistanciaRegata.QuinceKilometros => "15K",
                DistanciaRegata.VeintiDosKilometros => "22K",
                DistanciaRegata.VeintiCincoKilometros => "25K",
                DistanciaRegata.TreintaDosKilometros => "32K",
                _ => distancia.ToString()
            };
        }

        private decimal GetDistanciaEnMetros(DistanciaRegata distancia)
        {
            return distancia switch
            {
                DistanciaRegata.DoscientosMetros => 200,
                DistanciaRegata.TrecientosCincuentaMetros => 350,
                DistanciaRegata.QuatroCientosMetros => 400,
                DistanciaRegata.QuinientosMetros => 500,
                DistanciaRegata.MilMetros => 1000,
                DistanciaRegata.DosKilometros => 2000,
                DistanciaRegata.TresKilometros => 3000,
                DistanciaRegata.CincoKilometros => 5000,
                DistanciaRegata.DiezKilometros => 10000,
                DistanciaRegata.QuinceKilometros => 15000,
                DistanciaRegata.VeintiDosKilometros => 22000,
                DistanciaRegata.VeintiCincoKilometros => 25000,
                DistanciaRegata.TreintaDosKilometros => 32000,
                _ => 0
            };
        }

        private string GetTipoDistancia(DistanciaRegata distancia)
        {
            var metros = GetDistanciaEnMetros(distancia);
            return metros switch
            {
                <= 500 => "Sprint",
                <= 2000 => "Media Distancia",
                _ => "Larga Distancia"
            };
        }
    }
}
