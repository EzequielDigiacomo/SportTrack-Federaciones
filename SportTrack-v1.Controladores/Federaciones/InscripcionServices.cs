using Microsoft.EntityFrameworkCore;
using SportTrack.AccessDatos;
using SportTrack_v1.Entidades.Entidades;
using SportTrack_v1.Entidades.DTOs.Inscripcion;
using SportTrack_v1.Entidades.DTOs.AtletaFederado;
using SportTrack_v1.Entidades.DTOs.Evento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SportTrack_v1.Controladores.Federaciones;
using Microsoft.AspNetCore.Mvc;

namespace SIGDEF.API.Services
{
    public class InscripcionServices : IInscripcionServices
    {
        private readonly SportTrackDbContext _context;

        public InscripcionServices(SportTrackDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult<IEnumerable<InscripcionDto>>> GetInscripciones()
        {
            try
            {
                var inscripciones = await _context.Inscripciones
                    .Include(i => i.AtletaFederado)
                        .ThenInclude(a => a.Participante)
                    .Include(i => i.AtletaFederado)
                        .ThenInclude(a => a.Club)
                    .Include(i => i.EventoPrueba)
                        .ThenInclude(ep => ep.Evento)
                    .Include(i => i.EventoPrueba)
                        .ThenInclude(ep => ep.Prueba)
                    .Select(i => new InscripcionDto
                    {
                        IdInscripcion = i.IdInscripcion,
                        ParticipanteId = i.ParticipanteId,
                        IdEvento = i.EventoPrueba.Evento.IdEvento,
                        IdEventoPrueba = i.IdEventoPrueba,
                        FechaInscripcion = i.FechaInscripcion,
                        NombreAtleta = i.AtletaFederado.Participante.Nombre + " " + i.AtletaFederado.Participante.Apellido,
                        NombreEvento = i.EventoPrueba.Evento.Nombre,
                        DetallePrueba = $"{i.EventoPrueba.Prueba.Distancia} - {i.EventoPrueba.Prueba.TipoBote} - {i.EventoPrueba.Prueba.CategoriaEdad} - {i.EventoPrueba.Prueba.SexoCompetencia}",
                        NombreClub = i.AtletaFederado.Club.Nombre,
                        FechaInicioEvento = i.EventoPrueba.Evento.FechaInicio,
                        FechaFinEvento = i.EventoPrueba.Evento.FechaFin
                    })
                    .ToListAsync();

                return new OkObjectResult(inscripciones);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<InscripcionDetailDto>> GetInscripcion(int id)
        {
            try
            {
                var inscripcion = await _context.Inscripciones
                    .Include(i => i.AtletaFederado)
                        .ThenInclude(a => a.Participante)
                    .Include(i => i.AtletaFederado)
                        .ThenInclude(a => a.Club)
                    .Include(i => i.EventoPrueba)
                        .ThenInclude(ep => ep.Evento)
                    .Include(i => i.EventoPrueba)
                        .ThenInclude(ep => ep.Prueba)
                    .Where(i => i.IdInscripcion == id)
                    .Select(i => new InscripcionDetailDto
                    {
                        IdInscripcion = i.IdInscripcion,
                        ParticipanteId = i.ParticipanteId,
                        IdEvento = i.EventoPrueba.IdEvento,
                        IdEventoPrueba = i.IdEventoPrueba,
                        FechaInscripcion = i.FechaInscripcion,
                        AtletaFederado = new AtletaDto
                        {
                            ParticipanteId = i.AtletaFederado.ParticipanteId,
                            IdClub = i.AtletaFederado.IdClub,
                            EstadoPago = i.AtletaFederado.EstadoPago,
                            PerteneceSeleccion = i.AtletaFederado.PerteneceSeleccion,
                            Categoria = i.AtletaFederado.Categoria,
                            BecadoEnard = i.AtletaFederado.BecadoEnard,
                            BecadoSdn = i.AtletaFederado.BecadoSdn,
                            MontoBeca = i.AtletaFederado.MontoBeca,
                            PresentoAptoMedico = i.AtletaFederado.PresentoAptoMedico,
                            FechaAptoMedico = i.AtletaFederado.FechaAptoMedico,
                            NombrePersona = i.AtletaFederado.Participante.Nombre + " " + i.AtletaFederado.Participante.Apellido,
                            NombreClub = i.AtletaFederado.Club.Nombre
                        },
                        Evento = new EventoDto
                        {
                            IdEvento = i.EventoPrueba.Evento.IdEvento,
                            Nombre = i.EventoPrueba.Evento.Nombre,
                            FechaInicio = i.EventoPrueba.Evento.FechaInicio,
                            FechaFin = i.EventoPrueba.Evento.FechaFin,
                            CantidadInscripciones = i.EventoPrueba.Evento.Inscripciones.Count,
                            Estado = GetEstadoEvento(i.EventoPrueba.Evento.FechaInicio, i.EventoPrueba.Evento.FechaFin)
                        }
                    })
                    .FirstOrDefaultAsync();

                if (inscripcion == null)
                {
                    return new NotFoundResult();
                }

                return new OkObjectResult(inscripcion);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<IEnumerable<InscripcionDto>>> GetInscripcionesPorAtleta(int ParticipanteId)
        {
            try
            {
                var inscripciones = await _context.Inscripciones
                    .Include(i => i.AtletaFederado)
                        .ThenInclude(a => a.Participante)
                    .Include(i => i.AtletaFederado)
                        .ThenInclude(a => a.Club)
                    .Include(i => i.EventoPrueba)
                        .ThenInclude(ep => ep.Evento)
                    .Include(i => i.EventoPrueba)
                        .ThenInclude(ep => ep.Prueba)
                    .Where(i => i.ParticipanteId == ParticipanteId)
                    .Select(i => new InscripcionDto
                    {
                        IdInscripcion = i.IdInscripcion,
                        ParticipanteId = i.ParticipanteId,
                        IdEvento = i.EventoPrueba.IdEvento,
                        IdEventoPrueba = i.IdEventoPrueba,
                        FechaInscripcion = i.FechaInscripcion,
                        NombreAtleta = i.AtletaFederado.Participante.Nombre + " " + i.AtletaFederado.Participante.Apellido,
                        NombreEvento = i.EventoPrueba.Evento.Nombre,
                        DetallePrueba = $"{i.EventoPrueba.Prueba.Distancia} - {i.EventoPrueba.Prueba.TipoBote} - {i.EventoPrueba.Prueba.CategoriaEdad} - {i.EventoPrueba.Prueba.SexoCompetencia}",
                        NombreClub = i.AtletaFederado.Club.Nombre,
                        FechaInicioEvento = i.EventoPrueba.Evento.FechaInicio,
                        FechaFinEvento = i.EventoPrueba.Evento.FechaFin
                    })
                    .ToListAsync();

                return new OkObjectResult(inscripciones);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<IEnumerable<InscripcionDto>>> GetInscripcionesPorEvento(int idEvento)
        {
            try
            {
                var inscripciones = await _context.Inscripciones
                    .Include(i => i.AtletaFederado)
                        .ThenInclude(a => a.Participante)
                    .Include(i => i.AtletaFederado)
                        .ThenInclude(a => a.Club)
                    .Include(i => i.EventoPrueba)
                        .ThenInclude(ep => ep.Evento)
                    .Include(i => i.EventoPrueba)
                        .ThenInclude(ep => ep.Prueba)
                    .Where(i => i.EventoPrueba.IdEvento == idEvento)
                    .Select(i => new InscripcionDto
                    {
                        IdInscripcion = i.IdInscripcion,
                        ParticipanteId = i.ParticipanteId,
                        IdEvento = i.EventoPrueba.IdEvento,
                        IdEventoPrueba = i.IdEventoPrueba,
                        FechaInscripcion = i.FechaInscripcion,
                        NombreAtleta = i.AtletaFederado.Participante.Nombre + " " + i.AtletaFederado.Participante.Apellido,
                        NombreEvento = i.EventoPrueba.Evento.Nombre,
                        DetallePrueba = $"{i.EventoPrueba.Prueba.Distancia} - {i.EventoPrueba.Prueba.TipoBote} - {i.EventoPrueba.Prueba.CategoriaEdad} - {i.EventoPrueba.Prueba.SexoCompetencia}",
                        NombreClub = i.AtletaFederado.Club.Nombre,
                        FechaInicioEvento = i.EventoPrueba.Evento.FechaInicio,
                        FechaFinEvento = i.EventoPrueba.Evento.FechaFin
                    })
                    .ToListAsync();

                return new OkObjectResult(inscripciones);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<InscripcionDto>> PostInscripcion(InscripcionCreateDto inscripcionCreateDto)
        {
            try
            {
                var atletaExists = await _context.AtletasFederados.AnyAsync(a => a.ParticipanteId == inscripcionCreateDto.ParticipanteId);
                if (!atletaExists)
                {
                    return new BadRequestResult();
                }

                var eventoPrueba = await _context.EventoPruebas
                    .Include(ep => ep.Evento)
                    .Include(ep => ep.Prueba)
                    .FirstOrDefaultAsync(ep => ep.IdEventoPrueba == inscripcionCreateDto.IdEventoPrueba);

                if (eventoPrueba == null)
                {
                    return new BadRequestResult();
                }

                if (eventoPrueba.Evento.FechaFin < DateTime.UtcNow)
                {
                    return new BadRequestResult();
                }

                var inscripcionExistente = await _context.Inscripciones
                    .AnyAsync(i => i.ParticipanteId == inscripcionCreateDto.ParticipanteId &&
                                  i.IdEventoPrueba == inscripcionCreateDto.IdEventoPrueba);

                if (inscripcionExistente)
                {
                    return new BadRequestResult();
                }

                var inscripcion = new Inscripcion
                {
                    ParticipanteId = inscripcionCreateDto.ParticipanteId,
                    IdEventoPrueba = inscripcionCreateDto.IdEventoPrueba,
                    FechaInscripcion = inscripcionCreateDto.FechaInscripcion
                };

                _context.Inscripciones.Add(inscripcion);
                await _context.SaveChangesAsync();

                await _context.Entry(inscripcion)
                    .Reference(i => i.AtletaFederado)
                    .LoadAsync();
                await _context.Entry(inscripcion.AtletaFederado)
                    .Reference(a => a.Participante)
                    .LoadAsync();
                await _context.Entry(inscripcion.AtletaFederado)
                    .Reference(a => a.Club)
                    .LoadAsync();
                await _context.Entry(inscripcion)
                    .Reference(i => i.EventoPrueba)
                    .LoadAsync();
                await _context.Entry(inscripcion.EventoPrueba)
                    .Reference(ep => ep.Evento)
                    .LoadAsync();
                await _context.Entry(inscripcion.EventoPrueba)
                    .Reference(ep => ep.Prueba)
                    .LoadAsync();

                var inscripcionDto = new InscripcionDto
                {
                    IdInscripcion = inscripcion.IdInscripcion,
                    ParticipanteId = inscripcion.ParticipanteId,
                    IdEvento = inscripcion.EventoPrueba.Evento.IdEvento,
                    IdEventoPrueba = inscripcion.IdEventoPrueba,
                    FechaInscripcion = inscripcion.FechaInscripcion,
                    NombreAtleta = inscripcion.AtletaFederado.Participante.Nombre + " " + inscripcion.AtletaFederado.Participante.Apellido,
                    NombreEvento = inscripcion.EventoPrueba.Evento.Nombre,
                    DetallePrueba = $"{inscripcion.EventoPrueba.Prueba.Distancia} - {inscripcion.EventoPrueba.Prueba.TipoBote}",
                    NombreClub = inscripcion.AtletaFederado.Club.Nombre,
                    FechaInicioEvento = inscripcion.EventoPrueba.Evento.FechaInicio,
                    FechaFinEvento = inscripcion.EventoPrueba.Evento.FechaFin
                };

                var result = new ObjectResult(inscripcionDto)
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

        public async Task<IActionResult> PutInscripcion(int id, InscripcionCreateDto inscripcionCreateDto)
        {
            try
            {
                var inscripcion = await _context.Inscripciones.FindAsync(id);
                if (inscripcion == null)
                {
                    return new NotFoundResult();
                }

                var atletaExists = await _context.AtletasFederados.AnyAsync(a => a.ParticipanteId == inscripcionCreateDto.ParticipanteId);
                if (!atletaExists)
                {
                    return new BadRequestResult();
                }

                var pruebaExists = await _context.EventoPruebas.AnyAsync(ep => ep.IdEventoPrueba == inscripcionCreateDto.IdEventoPrueba);
                if (!pruebaExists)
                {
                    return new BadRequestResult();
                }

                var inscripcionExistente = await _context.Inscripciones
                    .AnyAsync(i => i.ParticipanteId == inscripcionCreateDto.ParticipanteId &&
                                  i.IdEventoPrueba == inscripcionCreateDto.IdEventoPrueba &&
                                  i.IdInscripcion != id);

                if (inscripcionExistente)
                {
                    return new BadRequestResult();
                }

                inscripcion.ParticipanteId = inscripcionCreateDto.ParticipanteId;
                inscripcion.IdEventoPrueba = inscripcionCreateDto.IdEventoPrueba;
                inscripcion.FechaInscripcion = inscripcionCreateDto.FechaInscripcion;

                await _context.SaveChangesAsync();
                return new NoContentResult();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await InscripcionExistsAsync(id))
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

        public async Task<IActionResult> DeleteInscripcion(int id)
        {
            try
            {
                var inscripcion = await _context.Inscripciones.FindAsync(id);
                if (inscripcion == null)
                {
                    return new NotFoundResult();
                }

                _context.Inscripciones.Remove(inscripcion);
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

        private async Task<bool> InscripcionExistsAsync(int id)
        {
            return await _context.Inscripciones.AnyAsync(e => e.IdInscripcion == id);
        }

        private string GetEstadoEvento(DateTime fechaInicio, DateTime fechaFin)
        {
            var ahora = DateTime.UtcNow;

            if (fechaInicio > ahora)
                return "Próximo";
            else if (fechaInicio <= ahora && fechaFin >= ahora)
                return "Activo";
            else
                return "Finalizado";
        }
    }
}
