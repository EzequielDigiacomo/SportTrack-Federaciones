using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportTrack.AccessDatos;
using SportTrack_v1.Entidades.Entidades;
using SportTrack_v1.Entidades.DTOs.AtletaTutor;
using SportTrack_v1.Entidades.DTOs.AtletaFederado;
using SportTrack_v1.Entidades.DTOs.Tutor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SportTrack_v1.Controladores.Federaciones;

namespace SIGDEF.API.Services
{
    public class AtletaTutorServices : IAtletaTutorServices
    {
        private readonly SportTrackDbContext _context;

        public AtletaTutorServices(SportTrackDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult<IEnumerable<AtletaTutorDto>>> GetAtletasTutores()
        {
            try
            {
                var atletasTutores = await _context.AtletasTutores
                    .Include(at => at.AtletaFederado)
                        .ThenInclude(a => a.Participante)
                    .Include(at => at.AtletaFederado)
                        .ThenInclude(a => a.Club)
                    .Include(at => at.Tutor)
                        .ThenInclude(t => t.Participante)
                    .Select(at => new AtletaTutorDto
                    {
                        ParticipanteId = at.ParticipanteId,
                        IdTutor = at.IdTutor,
                        Parentesco = at.Parentesco,
                        NombreAtleta = at.AtletaFederado.Participante.Nombre + " " + at.AtletaFederado.Participante.Apellido,
                        NombreTutor = at.Tutor.Participante.Nombre + " " + at.Tutor.Participante.Apellido,
                        NombreClub = at.AtletaFederado.Club.Nombre
                    })
                    .ToListAsync();

                return new OkObjectResult(atletasTutores);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<IEnumerable<AtletaTutorDto>>> GetTutoresPorAtleta(int ParticipanteId)
        {
            try
            {
                var tutores = await _context.AtletasTutores
                    .Include(at => at.AtletaFederado)
                        .ThenInclude(a => a.Participante)
                    .Include(at => at.Tutor)
                        .ThenInclude(t => t.Participante)
                    .Where(at => at.ParticipanteId == ParticipanteId)
                    .Select(at => new AtletaTutorDto
                    {
                        ParticipanteId = at.ParticipanteId,
                        IdTutor = at.IdTutor,
                        Parentesco = at.Parentesco,
                        NombreAtleta = at.AtletaFederado.Participante.Nombre + " " + at.AtletaFederado.Participante.Apellido,
                        NombreTutor = at.Tutor.Participante.Nombre + " " + at.Tutor.Participante.Apellido
                    })
                    .ToListAsync();

                return new OkObjectResult(tutores);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<IEnumerable<AtletaTutorDto>>> GetAtletasPorTutor(int idTutor)
        {
            try
            {
                var AtletasFederados = await _context.AtletasTutores
                    .Include(at => at.AtletaFederado)
                        .ThenInclude(a => a.Participante)
                    .Include(at => at.AtletaFederado)
                        .ThenInclude(a => a.Club)
                    .Include(at => at.Tutor)
                        .ThenInclude(t => t.Participante)
                    .Where(at => at.IdTutor == idTutor)
                    .Select(at => new AtletaTutorDto
                    {
                        ParticipanteId = at.ParticipanteId,
                        IdTutor = at.IdTutor,
                        Parentesco = at.Parentesco,
                        NombreAtleta = at.AtletaFederado.Participante.Nombre + " " + at.AtletaFederado.Participante.Apellido,
                        NombreTutor = at.Tutor.Participante.Nombre + " " + at.Tutor.Participante.Apellido,
                        NombreClub = at.AtletaFederado.Club.Nombre
                    })
                    .ToListAsync();

                return new OkObjectResult(AtletasFederados);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<AtletaTutorDetailDto>> GetAtletaTutor(int ParticipanteId, int idTutor)
        {
            try
            {
                var atletaTutor = await _context.AtletasTutores
                    .Include(at => at.AtletaFederado)
                        .ThenInclude(a => a.Participante)
                    .Include(at => at.AtletaFederado)
                        .ThenInclude(a => a.Club)
                    .Include(at => at.Tutor)
                        .ThenInclude(t => t.Participante)
                    .Where(at => at.ParticipanteId == ParticipanteId && at.IdTutor == idTutor)
                    .Select(at => new AtletaTutorDetailDto
                    {
                        ParticipanteId = at.ParticipanteId,
                        IdTutor = at.IdTutor,
                        Parentesco = at.Parentesco,
                        AtletaFederado = new AtletaDto
                        {
                            ParticipanteId = at.AtletaFederado.ParticipanteId,
                            IdClub = at.AtletaFederado.IdClub,
                            EstadoPago = at.AtletaFederado.EstadoPago,
                            PerteneceSeleccion = at.AtletaFederado.PerteneceSeleccion,
                            Categoria = at.AtletaFederado.Categoria,
                            NombrePersona = at.AtletaFederado.Participante.Nombre + " " + at.AtletaFederado.Participante.Apellido,
                            NombreClub = at.AtletaFederado.Club.Nombre
                        },
                        Tutor = new TutorDto
                        {
                            ParticipanteId = at.Tutor.ParticipanteId,
                            TipoTutor = at.Tutor.TipoTutor,
                            NombrePersona = at.Tutor.Participante.Nombre + " " + at.Tutor.Participante.Apellido
                        }
                    })
                    .FirstOrDefaultAsync();

                if (atletaTutor == null)
                {
                    return new NotFoundResult();
                }

                return new OkObjectResult(atletaTutor);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<AtletaTutorDto>> PostAtletaTutor(AtletaTutorCreateDto atletaTutorCreateDto)
        {
            try
            {
                var atletaExists = await _context.AtletasFederados.AnyAsync(a => a.ParticipanteId == atletaTutorCreateDto.ParticipanteId);
                if (!atletaExists)
                {
                    return new BadRequestResult();
                }

                var tutorExists = await _context.Tutores.AnyAsync(t => t.ParticipanteId == atletaTutorCreateDto.IdTutor);
                if (!tutorExists)
                {
                    return new BadRequestResult();
                }

                var relationExists = await _context.AtletasTutores
                    .AnyAsync(at => at.ParticipanteId == atletaTutorCreateDto.ParticipanteId && at.IdTutor == atletaTutorCreateDto.IdTutor);

                if (relationExists)
                {
                    return new BadRequestResult();
                }

                var atletaTutor = new AtletaTutor
                {
                    ParticipanteId = atletaTutorCreateDto.ParticipanteId,
                    IdTutor = atletaTutorCreateDto.IdTutor,
                    Parentesco = atletaTutorCreateDto.Parentesco
                };

                _context.AtletasTutores.Add(atletaTutor);
                await _context.SaveChangesAsync();

                await _context.Entry(atletaTutor)
                    .Reference(at => at.AtletaFederado)
                    .LoadAsync();
                await _context.Entry(atletaTutor.AtletaFederado)
                    .Reference(a => a.Participante)
                    .LoadAsync();
                await _context.Entry(atletaTutor.AtletaFederado)
                    .Reference(a => a.Club)
                    .LoadAsync();
                await _context.Entry(atletaTutor)
                    .Reference(at => at.Tutor)
                    .LoadAsync();
                await _context.Entry(atletaTutor.Tutor)
                    .Reference(t => t.Participante)
                    .LoadAsync();

                var atletaTutorDto = new AtletaTutorDto
                {
                    ParticipanteId = atletaTutor.ParticipanteId,
                    IdTutor = atletaTutor.IdTutor,
                    Parentesco = atletaTutor.Parentesco,
                    NombreAtleta = atletaTutor.AtletaFederado.Participante.Nombre + " " + atletaTutor.AtletaFederado.Participante.Apellido,
                    NombreTutor = atletaTutor.Tutor.Participante.Nombre + " " + atletaTutor.Tutor.Participante.Apellido,
                    NombreClub = atletaTutor.AtletaFederado.Club.Nombre
                };

                var result = new ObjectResult(atletaTutorDto)
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

        public async Task<IActionResult> PutAtletaTutor(int ParticipanteId, int idTutor, AtletaTutorCreateDto atletaTutorCreateDto)
        {
            try
            {
                if (ParticipanteId != atletaTutorCreateDto.ParticipanteId || idTutor != atletaTutorCreateDto.IdTutor)
                {
                    return new BadRequestResult();
                }

                var atletaTutor = await _context.AtletasTutores
                    .FirstOrDefaultAsync(at => at.ParticipanteId == ParticipanteId && at.IdTutor == idTutor);

                if (atletaTutor == null)
                {
                    return new NotFoundResult();
                }

                atletaTutor.Parentesco = atletaTutorCreateDto.Parentesco;

                await _context.SaveChangesAsync();

                return new NoContentResult();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AtletaTutorExistsAsync(ParticipanteId, idTutor))
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

        public async Task<IActionResult> DeleteAtletaTutor(int ParticipanteId, int idTutor)
        {
            try
            {
                var atletaTutor = await _context.AtletasTutores
                    .FirstOrDefaultAsync(at => at.ParticipanteId == ParticipanteId && at.IdTutor == idTutor);

                if (atletaTutor == null)
                {
                    return new NotFoundResult();
                }

                _context.AtletasTutores.Remove(atletaTutor);
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

        private async Task<bool> AtletaTutorExistsAsync(int ParticipanteId, int idTutor)
        {
            return await _context.AtletasTutores.AnyAsync(e => e.ParticipanteId == ParticipanteId && e.IdTutor == idTutor);
        }
    }
}
