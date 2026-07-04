using Microsoft.EntityFrameworkCore;
using SportTrack.AccessDatos;
using SportTrack_v1.Entidades.DTOs.Participante;
using SportTrack_v1.Entidades.DTOs.Usuario;
using SportTrack_v1.Entidades.DTOs.DelegadoClub;
using SportTrack_v1.Entidades.DTOs.Entrenador;
using SportTrack_v1.Entidades.DTOs.Tutor;
using SportTrack_v1.Entidades.DTOs.AtletaFederado;
using SportTrack_v1.Entidades.DTOs.PagoTransaccion;
using SportTrack_v1.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SportTrack_v1.Controladores.Federaciones;
using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Entidades.Entidades;

namespace SIGDEF.API.Services
{
    public class PersonaServices : IPersonaServices
    {
        private readonly SportTrackDbContext _context;

        public PersonaServices(SportTrackDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult<IEnumerable<PersonaDto>>> GetPersonas()
        {
            try
            {
                var Participantes = await _context.Participantes
                    .Select(p => new PersonaDto
                    {
                        ParticipanteId = p.ParticipanteId,
                        Nombre = p.Nombre,
                        Apellido = p.Apellido,
                        Documento = p.Dni,
                        FechaNacimiento = p.FechaNacimiento,
                        Email = p.Email,
                        Telefono = p.Telefono,
                        Direccion = p.Direccion,
                        Sexo = p.Sexo,
                        SexoDisplay = p.Sexo.ToString(),
                        Edad = CalcularEdad(p.FechaNacimiento),
                        NombreCompleto = p.Nombre + " " + p.Apellido,
                        TipoPersona = GetTipoPersona(p)
                    })
                    .ToListAsync();

                return new OkObjectResult(Participantes);
            }
            catch (Exception ex)
            {
                return new ObjectResult(new
                {
                    error = ex.Message,
                    innerError = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace
                })
                {
                    StatusCode = 500
                };
            }
        }

        public async Task<ActionResult<PersonaDetailDto>> GetPersona(int id)
        {
            try
            {
                var Participante = await _context.Participantes
                    .Include(p => p.Usuario)
                    .Include(p => p.DelegadoClub)
                    .Include(p => p.Entrenador)
                    .Include(p => p.Tutor)
                    .Include(p => p.AtletaFederado)
                    .Include(p => p.Pagos)
                    .Where(p => p.ParticipanteId == id)
                    .Select(p => new PersonaDetailDto
                    {
                        ParticipanteId = p.ParticipanteId,
                        Nombre = p.Nombre,
                        Apellido = p.Apellido,
                        Documento = p.Dni,
                        FechaNacimiento = p.FechaNacimiento,
                        Email = p.Email,
                        Telefono = p.Telefono,
                        Direccion = p.Direccion,
                        Sexo = p.Sexo,
                        SexoDisplay = p.Sexo.ToString(),
                        Usuario = p.Usuario != null ? new UsuarioDto
                        {
                            ParticipanteId = p.Usuario.ParticipanteId,
                            Username = p.Usuario.Username,
                            EstaActivo = p.Usuario.EstaActivo,
                            FechaCreacion = p.Usuario.FechaCreacion,
                            UltimoAcceso = p.Usuario.UltimoAcceso
                        } : null,
                        DelegadoClub = p.DelegadoClub != null ? new DelegadoClubDto
                        {
                            ParticipanteId = p.DelegadoClub.ParticipanteId,
                            IdRol = p.DelegadoClub.IdRol,
                            IdFederacion = p.DelegadoClub.IdFederacion
                        } : null,
                        Entrenador = p.Entrenador != null ? new EntrenadorDto
                        {
                            ParticipanteId = p.Entrenador.ParticipanteId,
                            IdClub = p.Entrenador.IdClub ?? 0,
                            PerteneceSeleccion = p.Entrenador.PerteneceSeleccion == true,
                            CategoriaSeleccion = p.Entrenador.CategoriaSeleccion,
                            BecadoEnard = p.Entrenador.BecadoEnard == true,
                            BecadoSdn = p.Entrenador.BecadoSdn == true,
                            MontoBeca = p.Entrenador.MontoBeca ?? 0,
                            PresentoAptoMedico = p.Entrenador.PresentoAptoMedico == true
                        } : null,
                        Tutor = p.Tutor != null ? new TutorDto
                        {
                            ParticipanteId = p.Tutor.ParticipanteId,
                            TipoTutor = p.Tutor.TipoTutor
                        } : null,
                        AtletaFederado = p.AtletaFederado != null ? new AtletaDto
                        {
                            ParticipanteId = p.AtletaFederado.ParticipanteId,
                            IdClub = p.AtletaFederado.IdClub,
                            EstadoPago = p.AtletaFederado.EstadoPago,
                            PerteneceSeleccion = p.AtletaFederado.PerteneceSeleccion,
                            Categoria = p.AtletaFederado.Categoria,
                            BecadoEnard = p.AtletaFederado.BecadoEnard,
                            BecadoSdn = p.AtletaFederado.BecadoSdn,
                            MontoBeca = p.AtletaFederado.MontoBeca,
                            PresentoAptoMedico = p.AtletaFederado.PresentoAptoMedico,
                            FechaAptoMedico = p.AtletaFederado.FechaAptoMedico
                        } : null,
                        Pagos = p.Pagos.Select(pa => new PagoTransaccionDto
                        {
                            IdPago = pa.IdPago,
                            Concepto = pa.Concepto,
                            Monto = pa.Monto,
                            Estado = pa.Estado,
                            FechaCreacion = pa.FechaCreacion,
                            FechaAprobacion = pa.FechaAprobacion,
                            ParticipanteId = pa.ParticipanteId,
                            IdClub = pa.IdClub,
                            IdMercadoPago = pa.IdMercadoPago
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (Participante == null)
                {
                    return new NotFoundResult();
                }

                return new OkObjectResult(Participante);
            }
            catch (Exception ex) { return new ObjectResult(new { error = ex.Message, inner = ex.InnerException?.Message }) { StatusCode = 500 }; }
        }

        public async Task<ActionResult<PersonaDto>> GetPersonaByDocumento(string documento)
        {
            try
            {
                var p = await _context.Participantes
                    .Include(x => x.Usuario)
                    .Include(x => x.DelegadoClub)
                    .Include(x => x.Entrenador)
                    .Include(x => x.Tutor)
                    .Include(x => x.AtletaFederado)
                    .Where(x => x.Dni == documento)
                    .FirstOrDefaultAsync();

                if (p == null)
                {
                    return new NotFoundResult();
                }

                var personaDto = new PersonaDto
                {
                    ParticipanteId = p.ParticipanteId,
                    Nombre = p.Nombre,
                    Apellido = p.Apellido,
                    Documento = p.Dni,
                    FechaNacimiento = p.FechaNacimiento,
                    Email = p.Email,
                    Telefono = p.Telefono,
                    Direccion = p.Direccion,
                    Sexo = p.Sexo,
                    SexoDisplay = p.Sexo.ToString(),
                    Edad = CalcularEdad(p.FechaNacimiento),
                    NombreCompleto = p.Nombre + " " + p.Apellido,
                    TipoPersona = GetTipoPersona(p)
                };

                return new OkObjectResult(personaDto);
            }
            catch (Exception ex) { return new ObjectResult(new { error = ex.Message, inner = ex.InnerException?.Message }) { StatusCode = 500 }; }
        }

        public async Task<ActionResult<PersonaDto>> PostPersona(PersonaCreateDto personaCreateDto)
        {
            try
            {
                var documentoExists = await _context.Participantes.AnyAsync(p => p.Dni == personaCreateDto.Dni);
                if (documentoExists)
                {
                    return new BadRequestResult();
                }

                if (!Enum.IsDefined(typeof(Sexo), personaCreateDto.Sexo))
                {
                    return new BadRequestResult();
                }

                var fechaNacimientoUtc = DateTime.SpecifyKind(personaCreateDto.FechaNacimiento, DateTimeKind.Utc);

                if (fechaNacimientoUtc > DateTime.UtcNow)
                {
                    return new BadRequestResult();
                }

                var edadMinima = DateTime.UtcNow.AddYears(-5);
                if (fechaNacimientoUtc > edadMinima)
                {
                    return new BadRequestResult();
                }

                var Participante = new Participante
                {
                    Nombre = personaCreateDto.Nombre,
                    Apellido = personaCreateDto.Apellido,
                    Documento = personaCreateDto.Dni,
                    FechaNacimiento = fechaNacimientoUtc,
                    Email = personaCreateDto.Email ?? string.Empty,
                    Telefono = personaCreateDto.Telefono ?? string.Empty,
                    Direccion = personaCreateDto.Direccion ?? string.Empty,
                    Sexo = personaCreateDto.Sexo
                };

                _context.Participantes.Add(Participante);
                await _context.SaveChangesAsync();

                var personaDto = new PersonaDto
                {
                    ParticipanteId = Participante.ParticipanteId,
                    Nombre = Participante.Nombre,
                    Apellido = Participante.Apellido,
                    Documento = Participante.Dni,
                    FechaNacimiento = Participante.FechaNacimiento,
                    Email = Participante.Email,
                    Telefono = Participante.Telefono,
                    Direccion = Participante.Direccion,
                    Sexo = Participante.Sexo,
                    SexoDisplay = Participante.Sexo.ToString(),
                    Edad = CalcularEdad(Participante.FechaNacimiento),
                    NombreCompleto = Participante.Nombre + " " + Participante.Apellido,
                    TipoPersona = "Participante Base"
                };

                var result = new ObjectResult(personaDto)
                {
                    StatusCode = 201
                };
                return result;
            }
            catch (DbUpdateException dbEx) { return new ObjectResult(new { error = "Error de base de datos", detail = dbEx.Message, inner = dbEx.InnerException?.Message }) { StatusCode = 500 }; }
            catch (Exception ex) { return new ObjectResult(new { error = ex.Message, inner = ex.InnerException?.Message }) { StatusCode = 500 }; }
        }

        public async Task<IActionResult> PutPersona(int id, PersonaCreateDto personaCreateDto)
        {
            try
            {
                var Participante = await _context.Participantes.FindAsync(id);
                if (Participante == null)
                {
                    return new NotFoundResult();
                }

                var documentoExists = await _context.Participantes
                    .AnyAsync(p => p.Dni == personaCreateDto.Documento && p.Id != id);
                if (documentoExists)
                {
                    return new BadRequestResult();
                }

                if (!Enum.IsDefined(typeof(Sexo), personaCreateDto.Sexo))
                {
                    return new BadRequestResult();
                }

                var fechaNacimientoUtc = DateTime.SpecifyKind(personaCreateDto.FechaNacimiento, DateTimeKind.Utc);

                if (fechaNacimientoUtc > DateTime.UtcNow)
                {
                    return new BadRequestResult();
                }

                Participante.Nombre = personaCreateDto.Nombre;
                Participante.Apellido = personaCreateDto.Apellido;
                Participante.Dni = personaCreateDto.Documento;
                Participante.FechaNacimiento = fechaNacimientoUtc;
                Participante.Email = personaCreateDto.Email ?? string.Empty;
                Participante.Telefono = personaCreateDto.Telefono ?? string.Empty;
                Participante.Direccion = personaCreateDto.Direccion ?? string.Empty;
                Participante.Sexo = personaCreateDto.Sexo;

                await _context.SaveChangesAsync();
                return new NoContentResult();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await PersonaExistsAsync(id))
                {
                    return new NotFoundResult();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex) { return new ObjectResult(new { error = ex.Message, inner = ex.InnerException?.Message }) { StatusCode = 500 }; }
        }

        public async Task<IActionResult> DeletePersona(int id)
        {
            try
            {
                var Participante = await _context.Participantes.FindAsync(id);
                if (Participante == null)
                {
                    return new NotFoundResult();
                }

                _context.Participantes.Remove(Participante);
                await _context.SaveChangesAsync();

                return new NoContentResult();
            }
            catch (DbUpdateException dbEx) { return new ObjectResult(new { error = "Error de base de datos", detail = dbEx.Message, inner = dbEx.InnerException?.Message }) { StatusCode = 500 }; }
            catch (Exception ex) { return new ObjectResult(new { error = ex.Message, inner = ex.InnerException?.Message }) { StatusCode = 500 }; }
        }

        private async Task<bool> PersonaExistsAsync(int id)
        {
            return await _context.Participantes.AnyAsync(e => e.Id == id);
        }

        private static int CalcularEdad(DateTime fechaNacimiento)
        {
            var hoy = DateTime.Today;
            var edad = hoy.Year - fechaNacimiento.Year;
            if (fechaNacimiento.Date > hoy.AddYears(-edad)) edad--;
            return edad;
        }

        private static string GetTipoPersona(Participante Participante)
        {
            if (Participante.AtletaFederado != null) return "AtletaFederado";
            if (Participante.Entrenador != null) return "Entrenador";
            if (Participante.Tutor != null) return "Tutor";
            if (Participante.DelegadoClub != null) return "DelegadoClub";
            return "Participante Base";
        }
    }
}


