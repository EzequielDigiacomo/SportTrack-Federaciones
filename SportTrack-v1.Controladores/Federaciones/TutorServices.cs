using Microsoft.EntityFrameworkCore;
using SportTrack.AccessDatos;
using SportTrack_v1.Entidades.Entidades;
using SportTrack_v1.Entidades.DTOs.Tutor;
using SportTrack_v1.Entidades.DTOs.Participante;
using SportTrack_v1.Entidades.DTOs.AtletaTutor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SportTrack_v1.Controladores.Federaciones;
using Microsoft.AspNetCore.Mvc;

namespace SIGDEF.API.Services
{
    public class TutorServices : ITutorServices
    {
        private readonly SportTrackDbContext _context;

        public TutorServices(SportTrackDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult<IEnumerable<TutorDto>>> GetTutores()
        {
            try
            {
                var tutores = await _context.Tutores
                    .Include(t => t.Participante)
                    .Include(t => t.AtletasTutores)
                    .Select(t => new TutorDto
                    {
                        ParticipanteId = t.ParticipanteId,
                        TipoTutor = t.TipoTutor,
                        NombrePersona = t.Participante.Nombre + " " + t.Participante.Apellido,
                        Documento = t.Participante.Dni,
                        Telefono = t.Participante.Telefono,
                        Email = t.Participante.Email,
                        CantidadAtletas = t.AtletasTutores.Count
                    })
                    .ToListAsync();

                return new OkObjectResult(tutores);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<TutorDetailDto>> GetTutor(int id)
        {
            try
            {
                var tutor = await _context.Tutores
                    .Include(t => t.Participante)
                    .Include(t => t.AtletasTutores)
                        .ThenInclude(at => at.AtletaFederado)
                        .ThenInclude(a => a.Participante)
                    .Include(t => t.AtletasTutores)
                        .ThenInclude(at => at.AtletaFederado)
                        .ThenInclude(a => a.Club)
                    .Where(t => t.ParticipanteId == id)
                    .Select(t => new TutorDetailDto
                    {
                        ParticipanteId = t.ParticipanteId,
                        TipoTutor = t.TipoTutor,
                        Participante = new PersonaDto
                        {
                            ParticipanteId = t.Participante.ParticipanteId,
                            Nombre = t.Participante.Nombre,
                            Apellido = t.Participante.Apellido,
                            Documento = t.Participante.Dni,
                            FechaNacimiento = t.Participante.FechaNacimiento,
                            Email = t.Participante.Email,
                            Telefono = t.Participante.Telefono,
                            Direccion = t.Participante.Direccion
                        },
                        AtletasTutores = t.AtletasTutores.Select(at => new AtletaTutorDto
                        {
                            ParticipanteId = at.ParticipanteId,
                            IdTutor = at.IdTutor,
                            Parentesco = at.Parentesco,
                            NombreAtleta = at.AtletaFederado.Participante.Nombre + " " + at.AtletaFederado.Participante.Apellido,
                            NombreClub = at.AtletaFederado.Club.Nombre.ToString()
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (tutor == null)
                {
                    return new NotFoundResult();
                }

                return new OkObjectResult(tutor);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<IEnumerable<TutorDto>>> GetTutoresPorTipo(string tipoTutor)
        {
            try
            {
                var tutores = await _context.Tutores
                    .Include(t => t.Participante)
                    .Include(t => t.AtletasTutores)
                    .Where(t => t.TipoTutor.ToLower() == tipoTutor.ToLower())
                    .Select(t => new TutorDto
                    {
                        ParticipanteId = t.ParticipanteId,
                        TipoTutor = t.TipoTutor,
                        NombrePersona = t.Participante.Nombre + " " + t.Participante.Apellido,
                        Documento = t.Participante.Dni,
                        Telefono = t.Participante.Telefono,
                        Email = t.Participante.Email,
                        CantidadAtletas = t.AtletasTutores.Count
                    })
                    .ToListAsync();

                return new OkObjectResult(tutores);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<IEnumerable<TutorDto>>> GetTutoresSinAtletas()
        {
            try
            {
                var tutores = await _context.Tutores
                    .Include(t => t.Participante)
                    .Include(t => t.AtletasTutores)
                    .Where(t => !t.AtletasTutores.Any())
                    .Select(t => new TutorDto
                    {
                        ParticipanteId = t.ParticipanteId,
                        TipoTutor = t.TipoTutor,
                        NombrePersona = t.Participante.Nombre + " " + t.Participante.Apellido,
                        Documento = t.Participante.Dni,
                        Telefono = t.Participante.Telefono,
                        Email = t.Participante.Email,
                        CantidadAtletas = 0
                    })
                    .ToListAsync();

                return new OkObjectResult(tutores);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<IEnumerable<TutorDto>>> SearchTutores(string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    return new BadRequestResult();
                }

                var tutores = await _context.Tutores
                    .Include(t => t.Participante)
                    .Include(t => t.AtletasTutores)
                    .Where(t => t.Participante.Nombre.Contains(term) ||
                               t.Participante.Apellido.Contains(term) ||
                               t.Participante.Dni.Contains(term) ||
                               t.TipoTutor.Contains(term))
                    .Select(t => new TutorDto
                    {
                        ParticipanteId = t.ParticipanteId,
                        TipoTutor = t.TipoTutor,
                        NombrePersona = t.Participante.Nombre + " " + t.Participante.Apellido,
                        Documento = t.Participante.Dni,
                        Telefono = t.Participante.Telefono,
                        Email = t.Participante.Email,
                        CantidadAtletas = t.AtletasTutores.Count
                    })
                    .ToListAsync();

                return new OkObjectResult(tutores);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<IEnumerable<string>>> GetTiposTutor()
        {
            try
            {
                var tipos = await _context.Tutores
                    .Select(t => t.TipoTutor)
                    .Distinct()
                    .ToListAsync();

                return new OkObjectResult(tipos);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<TutorDto>> PostTutor(TutorCreateDto tutorCreateDto)
        {
            try
            {
                var personaExists = await _context.Participantes.AnyAsync(p => p.ParticipanteId == tutorCreateDto.ParticipanteId);
                if (!personaExists)
                {
                    return new BadRequestResult();
                }

                var tutorExists = await _context.Tutores.AnyAsync(t => t.ParticipanteId == tutorCreateDto.ParticipanteId);
                if (tutorExists)
                {
                    return new BadRequestResult();
                }

                var tieneOtroRol = await _context.AtletasFederados.AnyAsync(a => a.ParticipanteId == tutorCreateDto.ParticipanteId) ||
                                  await _context.Entrenadores.AnyAsync(e => e.ParticipanteId == tutorCreateDto.ParticipanteId) ||
                                  await _context.DelegadosClub.AnyAsync(d => d.ParticipanteId == tutorCreateDto.ParticipanteId);

                if (tieneOtroRol)
                {
                    return new BadRequestResult();
                }

                var tutor = new Tutor
                {
                    ParticipanteId = tutorCreateDto.ParticipanteId,
                    TipoTutor = tutorCreateDto.TipoTutor
                };

                _context.Tutores.Add(tutor);
                await _context.SaveChangesAsync();

                await _context.Entry(tutor)
                    .Reference(t => t.Participante)
                    .LoadAsync();

                var tutorDto = new TutorDto
                {
                    ParticipanteId = tutor.ParticipanteId,
                    TipoTutor = tutor.TipoTutor,
                    NombrePersona = tutor.Participante.Nombre + " " + tutor.Participante.Apellido,
                    Documento = tutor.Participante.Dni,
                    Telefono = tutor.Participante.Telefono,
                    Email = tutor.Participante.Email,
                    CantidadAtletas = 0
                };

                var result = new ObjectResult(tutorDto)
                {
                    StatusCode = 201
                };
                return result;
            }
            catch (DbUpdateException dbEx)
            {
                return new StatusCodeResult(500);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<IActionResult> PutTutor(int id, TutorCreateDto tutorCreateDto)
        {
            try
            {
                if (id != tutorCreateDto.ParticipanteId)
                {
                    return new BadRequestResult();
                }

                var tutor = await _context.Tutores.FindAsync(id);
                if (tutor == null)
                {
                    return new NotFoundResult();
                }

                tutor.TipoTutor = tutorCreateDto.TipoTutor;
                await _context.SaveChangesAsync();
                return new NoContentResult();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TutorExistsAsync(id))
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

        public async Task<IActionResult> DeleteTutor(int id)
        {
            try
            {
                var tutor = await _context.Tutores
                    .Include(t => t.AtletasTutores)
                    .FirstOrDefaultAsync(t => t.ParticipanteId == id);

                if (tutor == null)
                {
                    return new NotFoundResult();
                }

                if (tutor.AtletasTutores.Any())
                {
                    return new BadRequestResult();
                }

                _context.Tutores.Remove(tutor);
                await _context.SaveChangesAsync();
                return new NoContentResult();
            }
            catch (DbUpdateException dbEx)
            {
                return new StatusCodeResult(500);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        private async Task<bool> TutorExistsAsync(int id)
        {
            return await _context.Tutores.AnyAsync(e => e.ParticipanteId == id);
        }
    }
}
