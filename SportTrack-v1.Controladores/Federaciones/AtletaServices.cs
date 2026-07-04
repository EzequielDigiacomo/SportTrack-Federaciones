using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportTrack.AccessDatos;
using SportTrack_v1.Controladores.Federaciones;
using SIGDEF.DTOs;
using SportTrack_v1.Entidades.Entidades;
using SportTrack_v1.Entidades.DTOs.AtletaFederado;
using SportTrack_v1.Entidades.DTOs.AtletaTutor;
using SportTrack_v1.Entidades.DTOs.Base;
using SportTrack_v1.Entidades.DTOs.Club;
using SportTrack_v1.Entidades.DTOs.Inscripcion;
using SportTrack_v1.Entidades.DTOs.Participante;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SportTrack_v1.Entidades.Enums;

namespace SIGDEF.API.Services
{
    public class AtletaServices : IAtletaServices
    {
        private readonly SportTrackDbContext _context;
        private readonly ITenantProvider _tenantProvider;

        public AtletaServices(SportTrackDbContext context, ITenantProvider tenantProvider)
        {
            _context = context;
            _tenantProvider = tenantProvider;
        }

        // -------------------------------------------------
        // GET: Obtener todos los AtletasFederados
        // -------------------------------------------------
        public async Task<ActionResult<IEnumerable<AtletaDetailDto>>> GetAtletas()
        {
            try
            {
                var query = _context.AtletasFederados.AsQueryable();
                var fedId = _tenantProvider.GetFederacionId();
                if (fedId.HasValue)
                {
                    query = query.Where(a => a.IdFederacion == fedId.Value);
                }
                var clubId = _tenantProvider.GetClubId();
                if (clubId.HasValue)
                {
                    query = query.Where(a => a.IdClub == clubId.Value);
                }

                var AtletasFederados = await query
                    .Include(a => a.Participante)
                    .Include(a => a.Club)
                    .Include(a => a.Inscripciones)
                        .ThenInclude(i => i.EventoPrueba)
                    .Include(a => a.Tutores)
                        .ThenInclude(at => at.Tutor)
                        .ThenInclude(t => t.Participante)
                    .Select(a => new AtletaDetailDto
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
                        FechaCreacion = a.FechaCreacion,
                        Participante = new PersonaDto
                        {
                            ParticipanteId = a.Participante.ParticipanteId,
                            Nombre = a.Participante.Nombre,
                            Apellido = a.Participante.Apellido,
                            Documento = a.Participante.Dni,
                            FechaNacimiento = a.Participante.FechaNacimiento,
                            Email = a.Participante.Email,
                            Telefono = a.Participante.Telefono,
                            Direccion = a.Participante.Direccion
                        },
                        Club = a.Club != null ? new ClubDto
                        {
                            IdClub = a.Club.IdClub,
                            Nombre = a.Club.Nombre,
                            Siglas = a.Club.Siglas
                        } : null,
                        Inscripciones = a.Inscripciones.Select(i => new InscripcionDto
                        {
                            IdInscripcion = i.IdInscripcion,
                            IdEvento = i.IdEventoPrueba,
                            FechaInscripcion = i.FechaInscripcion,
                        }).ToList(),
                        Tutores = a.Tutores.Select(at => new AtletaTutorDto
                        {
                            ParticipanteId = at.ParticipanteId,
                            IdTutor = at.IdTutor,
                            Parentesco = at.Parentesco,
                            NombreTutor = (at.Tutor != null && at.Tutor.Participante != null) ? at.Tutor.Participante.Nombre + " " + at.Tutor.Participante.Apellido : "Tutor no encontrado"
                        }).ToList()
                    })
                    .ToListAsync();

                return new OkObjectResult(AtletasFederados);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        // -------------------------------------------------
        // GET: Obtener AtletasFederados paginados
        // -------------------------------------------------
        public async Task<ActionResult<PagedResponseDto<AtletaListDto>>> GetAtletasPaginadosAsync(PaginationParamsDto parameters)
        {
            try
            {
                var query = _context.AtletasFederados.AsQueryable();
                var fedId = _tenantProvider.GetFederacionId();
                if (fedId.HasValue)
                {
                    query = query.Where(a => a.IdFederacion == fedId.Value);
                }
                var clubId = _tenantProvider.GetClubId();
                if (clubId.HasValue)
                {
                    query = query.Where(a => a.IdClub == clubId.Value);
                }

                query = query
                    .AsNoTracking()
                    .Include(a => a.Participante)
                    .Include(a => a.Club)
                    .Include(a => a.Tutores)
                        .ThenInclude(at => at.Tutor)
                        .ThenInclude(t => t.Participante)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
                {
                    var search = parameters.SearchTerm.ToLower();
                    query = query.Where(a =>
                        a.Participante.Nombre.ToLower().Contains(search) ||
                        a.Participante.Apellido.ToLower().Contains(search) ||
                        a.Participante.Dni.ToLower().Contains(search) ||
                        (a.Club != null && a.Club.Nombre.ToLower().Contains(search))
                    );
                }

                var totalRecords = await query.CountAsync();

                var pagedData = await query
                    .OrderByDescending(a => a.ParticipanteId)
                    .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                    .Take(parameters.PageSize)
                    .Select(a => new AtletaListDto
                    {
                        ParticipanteId = a.ParticipanteId,
                        NombrePersona = (a.Participante.Nombre + " " + a.Participante.Apellido).Trim(),
                        Documento = a.Participante.Dni,
                        FechaNacimiento = a.Participante.FechaNacimiento,
                        NombreClub = a.Club != null ? a.Club.Nombre : "Agente Libre",
                        Categoria = a.Categoria,
                        PerteneceSeleccion = a.PerteneceSeleccion,
                        EstadoPago = a.EstadoPago,
                        FechaCreacion = a.FechaCreacion,
                        CantidadDocumentos = 0,
                        Edad = (DateTime.UtcNow.Year - a.Participante.FechaNacimiento.Year) -
                            (DateTime.UtcNow.DayOfYear < a.Participante.FechaNacimiento.DayOfYear ? 1 : 0),
                        TutorInfo = a.Tutores.Select(t => new TutorListDto
                        {
                            ParticipanteId = t.IdTutor,
                            Nombre = t.Tutor.Participante.Nombre,
                            Apellido = t.Tutor.Participante.Apellido,
                            Documento = t.Tutor.Participante.Dni,
                            Telefono = t.Tutor.Participante.Telefono
                        }).FirstOrDefault()
                    })
                    .ToListAsync();

                return new OkObjectResult(new PagedResponseDto<AtletaListDto>
                {
                    Data = pagedData,
                    PageNumber = parameters.PageNumber,
                    PageSize = parameters.PageSize,
                    TotalRecords = totalRecords,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / parameters.PageSize)
                });
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        // -------------------------------------------------
        // GET: Obtener AtletaFederado por ID
        // -------------------------------------------------
        public async Task<ActionResult<AtletaDetailDto>> GetAtleta(int id)
        {
            try
            {
                var AtletaFederado = await _context.AtletasFederados
                    .Include(a => a.Participante)
                    .Include(a => a.Club)
                    .Include(a => a.Inscripciones)
                        .ThenInclude(i => i.EventoPrueba)
                    .Include(a => a.Tutores)
                        .ThenInclude(at => at.Tutor)
                        .ThenInclude(t => t.Participante)
                    .Where(a => a.ParticipanteId == id)
                    .Select(a => new AtletaDetailDto
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
                        FechaCreacion = a.FechaCreacion,

                        Participante = new PersonaDto
                        {
                            ParticipanteId = a.Participante.ParticipanteId,
                            Nombre = a.Participante.Nombre,
                            Apellido = a.Participante.Apellido,
                            Documento = a.Participante.Dni,
                            FechaNacimiento = a.Participante.FechaNacimiento,
                            Email = a.Participante.Email,
                            Telefono = a.Participante.Telefono,
                            Direccion = a.Participante.Direccion
                        },
                        Club = a.Club != null ? new ClubDto
                        {
                            IdClub = a.Club.IdClub,
                            Nombre = a.Club.Nombre,
                            Siglas = a.Club.Siglas
                        } : null,

                        Inscripciones = a.Inscripciones.Select(i => new InscripcionDto
                        {
                            IdInscripcion = i.IdInscripcion,
                            IdEvento = i.IdEventoPrueba,
                            FechaInscripcion = i.FechaInscripcion,
                        }).ToList(),
                        Tutores = a.Tutores.Select(at => new AtletaTutorDto
                        {
                            ParticipanteId = at.ParticipanteId,
                            IdTutor = at.IdTutor,
                            Parentesco = at.Parentesco,
                            NombreTutor = (at.Tutor != null && at.Tutor.Participante != null) ? at.Tutor.Participante.Nombre + " " + at.Tutor.Participante.Apellido : "Tutor no encontrado"
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (AtletaFederado == null)
                {
                    return new NotFoundResult();
                }

                return new OkObjectResult(AtletaFederado);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        // -------------------------------------------------
        // POST: Crear nuevo AtletaFederado
        // -------------------------------------------------
        public async Task<ActionResult<AtletaDto>> PostAtleta(AtletaCreateDto atletaCreateDto)
        {
            try
            {
                // Validar existencia de Participante
                var personaExists = await _context.Participantes.AnyAsync(p => p.ParticipanteId == atletaCreateDto.ParticipanteId);
                if (!personaExists)
                {
                    return new BadRequestResult();
                }

                // Validar existencia de Club
                if (atletaCreateDto.IdClub.HasValue)
                {
                    var clubExists = await _context.Clubes.AnyAsync(c => c.IdClub == atletaCreateDto.IdClub.Value);
                    if (!clubExists)
                    {
                        return new BadRequestResult();
                    }
                }

                // Evitar duplicados
                var atletaExists = await _context.AtletasFederados.AnyAsync(a => a.ParticipanteId == atletaCreateDto.ParticipanteId);
                if (atletaExists)
                {
                    return new BadRequestResult();
                }

                // Convertir fecha de apto médico a UTC (si corresponde)
                DateTime? fechaAptoMedicoUtc = null;
                if (atletaCreateDto.FechaAptoMedico.HasValue)
                {
                    fechaAptoMedicoUtc = DateTime.SpecifyKind(atletaCreateDto.FechaAptoMedico.Value, DateTimeKind.Utc);
                }

                int? finalFedId = _tenantProvider.GetFederacionId();
                if (!finalFedId.HasValue && atletaCreateDto.IdFederacion.HasValue)
                {
                    finalFedId = atletaCreateDto.IdFederacion;
                }
                if (!finalFedId.HasValue && atletaCreateDto.IdClub.HasValue)
                {
                    var club = await _context.Clubes.FindAsync(atletaCreateDto.IdClub.Value);
                    finalFedId = club?.IdFederacion;
                }

                var AtletaFederado = new AtletaFederado
                {
                    ParticipanteId = atletaCreateDto.ParticipanteId,
                    IdClub = atletaCreateDto.IdClub,
                    IdFederacion = finalFedId,
                    EstadoPago = atletaCreateDto.EstadoPago,
                    PerteneceSeleccion = atletaCreateDto.PerteneceSeleccion,
                    Categoria = atletaCreateDto.Categoria,
                    BecadoEnard = atletaCreateDto.BecadoEnard,
                    BecadoSdn = atletaCreateDto.BecadoSdn,
                    MontoBeca = atletaCreateDto.MontoBeca,
                    PresentoAptoMedico = atletaCreateDto.PresentoAptoMedico,
                    FechaAptoMedico = fechaAptoMedicoUtc,
                };

                _context.AtletasFederados.Add(AtletaFederado);
                await _context.SaveChangesAsync();

                // Cargar relaciones para la respuesta
                await _context.Entry(AtletaFederado).Reference(a => a.Participante).LoadAsync();
                await _context.Entry(AtletaFederado).Reference(a => a.Club).LoadAsync();

                var atletaDto = new AtletaDto
                {
                    ParticipanteId = AtletaFederado.ParticipanteId,
                    IdClub = AtletaFederado.IdClub,
                    EstadoPago = AtletaFederado.EstadoPago,
                    PerteneceSeleccion = AtletaFederado.PerteneceSeleccion,
                    Categoria = AtletaFederado.Categoria,
                    BecadoEnard = AtletaFederado.BecadoEnard,
                    BecadoSdn = AtletaFederado.BecadoSdn,
                    MontoBeca = AtletaFederado.MontoBeca,
                    PresentoAptoMedico = AtletaFederado.PresentoAptoMedico,
                    FechaAptoMedico = AtletaFederado.FechaAptoMedico,
                    NombrePersona = AtletaFederado.Participante.Nombre + " " + AtletaFederado.Participante.Apellido,
                    NombreClub = AtletaFederado.Club != null ? AtletaFederado.Club.Nombre : "Agente Libre",
                    FechaCreacion = AtletaFederado.FechaCreacion
                };

                var result = new ObjectResult(atletaDto)
                {
                    StatusCode = 201
                };
                return result;
            }
            catch (DbUpdateException)
            {
                return new StatusCodeResult(500);
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        // -------------------------------------------------
        // POST: Registro Atómico de AtletaFederado (incluye Tutor si es menor)
        // -------------------------------------------------
        public async Task<ActionResult<AtletaDto>> PostAtletaFull(AtletaFullCreateDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Manejar Participante del AtletaFederado
                var personaAtleta = await _context.Participantes
                    .FirstOrDefaultAsync(p => p.Dni == dto.PersonaAtleta.Dni);
                
                if (personaAtleta == null)
                {
                    personaAtleta = new Participante
                    {
                        Nombre = dto.PersonaAtleta.Nombre,
                        Apellido = dto.PersonaAtleta.Apellido,
                        Documento = dto.PersonaAtleta.Dni,
                        FechaNacimiento = DateTime.SpecifyKind(dto.PersonaAtleta.FechaNacimiento, DateTimeKind.Utc),
                        Email = dto.PersonaAtleta.Email ?? "",
                        Telefono = dto.PersonaAtleta.Telefono ?? "",
                        Direccion = dto.PersonaAtleta.Direccion ?? "",
                        Sexo = dto.PersonaAtleta.Sexo
                    };
                    _context.Participantes.Add(personaAtleta);
                }
                else
                {
                    // Actualizar datos si ya existe
                    personaAtleta.Nombre = dto.PersonaAtleta.Nombre;
                    personaAtleta.Apellido = dto.PersonaAtleta.Apellido;
                    personaAtleta.FechaNacimiento = DateTime.SpecifyKind(dto.PersonaAtleta.FechaNacimiento, DateTimeKind.Utc);
                    personaAtleta.Email = dto.PersonaAtleta.Email ?? "";
                    personaAtleta.Telefono = dto.PersonaAtleta.Telefono ?? "";
                    personaAtleta.Direccion = dto.PersonaAtleta.Direccion ?? "";
                    personaAtleta.Sexo = dto.PersonaAtleta.Sexo;
                }
                await _context.SaveChangesAsync();

                int? finalFedId = _tenantProvider.GetFederacionId();
                if (!finalFedId.HasValue && dto.DatosDeportivos.IdFederacion.HasValue)
                {
                    finalFedId = dto.DatosDeportivos.IdFederacion;
                }
                if (!finalFedId.HasValue && dto.DatosDeportivos.IdClub.HasValue)
                {
                    var club = await _context.Clubes.FindAsync(dto.DatosDeportivos.IdClub.Value);
                    finalFedId = club?.IdFederacion;
                }

                // 2. Manejar AtletaFederado
                var AtletaFederado = await _context.AtletasFederados.FindAsync(personaAtleta.ParticipanteId);
                if (AtletaFederado == null)
                {
                    AtletaFederado = new AtletaFederado
                    {
                        ParticipanteId = personaAtleta.ParticipanteId,
                        IdClub = dto.DatosDeportivos.IdClub,
                        IdFederacion = finalFedId,
                        EstadoPago = dto.DatosDeportivos.EstadoPago,
                        PerteneceSeleccion = dto.DatosDeportivos.PerteneceSeleccion,
                        Categoria = dto.DatosDeportivos.Categoria,
                        BecadoEnard = dto.DatosDeportivos.BecadoEnard,
                        BecadoSdn = dto.DatosDeportivos.BecadoSdn,
                        MontoBeca = dto.DatosDeportivos.MontoBeca,
                        PresentoAptoMedico = dto.DatosDeportivos.PresentoAptoMedico,
                        FechaAptoMedico = dto.DatosDeportivos.FechaAptoMedico.HasValue 
                            ? DateTime.SpecifyKind(dto.DatosDeportivos.FechaAptoMedico.Value, DateTimeKind.Utc) 
                            : null
                    };
                    _context.AtletasFederados.Add(AtletaFederado);
                }
                else
                {
                    AtletaFederado.IdClub = dto.DatosDeportivos.IdClub;
                    AtletaFederado.IdFederacion = finalFedId;
                    AtletaFederado.EstadoPago = dto.DatosDeportivos.EstadoPago;
                    AtletaFederado.PerteneceSeleccion = dto.DatosDeportivos.PerteneceSeleccion;
                    AtletaFederado.Categoria = dto.DatosDeportivos.Categoria;
                    AtletaFederado.BecadoEnard = dto.DatosDeportivos.BecadoEnard;
                    AtletaFederado.BecadoSdn = dto.DatosDeportivos.BecadoSdn;
                    AtletaFederado.MontoBeca = dto.DatosDeportivos.MontoBeca;
                    AtletaFederado.PresentoAptoMedico = dto.DatosDeportivos.PresentoAptoMedico;
                    AtletaFederado.FechaAptoMedico = dto.DatosDeportivos.FechaAptoMedico.HasValue 
                            ? DateTime.SpecifyKind(dto.DatosDeportivos.FechaAptoMedico.Value, DateTimeKind.Utc) 
                            : null;
                }
                await _context.SaveChangesAsync();

                // 3. Manejar Tutor si es menor
                if (dto.EsMenor && dto.Tutor != null)
                {
                    int idPersonaTutor;
                    if (dto.Tutor.IdPersonaTutor.HasValue)
                    {
                        idPersonaTutor = dto.Tutor.IdPersonaTutor.Value;
                    }
                    else if (dto.Tutor.PersonaTutor != null)
                    {
                        var personaTutor = await _context.Participantes
                            .FirstOrDefaultAsync(p => p.Dni == dto.Tutor.PersonaTutor.Dni);
                        
                        if (personaTutor == null)
                        {
                            personaTutor = new Participante
                            {
                                Nombre = dto.Tutor.PersonaTutor.Nombre,
                                Apellido = dto.Tutor.PersonaTutor.Apellido,
                                Documento = dto.Tutor.PersonaTutor.Dni,
                                FechaNacimiento = DateTime.SpecifyKind(dto.Tutor.PersonaTutor.FechaNacimiento, DateTimeKind.Utc),
                                Email = dto.Tutor.PersonaTutor.Email ?? "",
                                Telefono = dto.Tutor.PersonaTutor.Telefono ?? "",
                                Direccion = dto.Tutor.PersonaTutor.Direccion ?? ""
                            };
                            _context.Participantes.Add(personaTutor);
                            await _context.SaveChangesAsync();
                        }
                        idPersonaTutor = personaTutor.ParticipanteId;
                    }
                    else
                    {
                        throw new Exception("Datos del tutor incompletos para registro de menor.");
                    }

                    // Asegurar registro de Tutor
                    var tutor = await _context.Tutores.FindAsync(idPersonaTutor);
                    if (tutor == null)
                    {
                        tutor = new Tutor 
                        { 
                            ParticipanteId = idPersonaTutor, 
                            TipoTutor = "Registrado vía AtletaFederado" 
                        };
                        _context.Tutores.Add(tutor);
                        await _context.SaveChangesAsync();
                    }

                    // Relación AtletaTutor
                    var relacion = await _context.AtletasTutores
                        .FirstOrDefaultAsync(at => at.ParticipanteId == AtletaFederado.ParticipanteId && at.IdTutor == idPersonaTutor);
                    
                    if (relacion == null)
                    {
                        relacion = new AtletaTutor
                        {
                            ParticipanteId = AtletaFederado.ParticipanteId,
                            IdTutor = idPersonaTutor,
                            Parentesco = (SportTrack_v1.Entidades.Enums.Parentesco)dto.Tutor.Parentesco
                        };
                        _context.AtletasTutores.Add(relacion);
                    }
                    else
                    {
                        relacion.Parentesco = (SportTrack_v1.Entidades.Enums.Parentesco)dto.Tutor.Parentesco;
                    }
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                // Reutilizar lógica de respuesta de GetAtleta por consistencia
                var response = await GetAtleta(AtletaFederado.ParticipanteId);
                
                // Mapear AtletaDetailDto a AtletaDto para coincidir con la firma
                if (response.Value is AtletaDetailDto detail)
                {
                    return new OkObjectResult(new AtletaDto
                    {
                        ParticipanteId = detail.ParticipanteId,
                        IdClub = detail.IdClub,
                        EstadoPago = detail.EstadoPago,
                        NombrePersona = detail.Participante.Nombre + " " + detail.Participante.Apellido,
                        NombreClub = detail.Club?.Nombre ?? "Agente Libre",
                        Categoria = detail.Categoria,
                        FechaCreacion = detail.FechaCreacion
                        // ... completar campos si es necesario
                    });
                }

                return response.Result ?? new StatusCodeResult(201);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ObjectResult(new { error = "Fallo el registro atómico", detail = ex.Message }) { StatusCode = 500 };
            }
        }

        // -------------------------------------------------
        // PUT: Actualizar AtletaFederado
        // -------------------------------------------------
        public async Task<IActionResult> PutAtleta(int id, AtletaCreateDto atletaCreateDto)
        {
            try
            {
                if (id != atletaCreateDto.ParticipanteId)
                {
                    return new BadRequestResult();
                }

                var AtletaFederado = await _context.AtletasFederados.FindAsync(id);
                if (AtletaFederado == null)
                {
                    return new NotFoundResult();
                }

                // Validar club
                if (atletaCreateDto.IdClub.HasValue)
                {
                    var clubExists = await _context.Clubes.AnyAsync(c => c.IdClub == atletaCreateDto.IdClub.Value);
                    if (!clubExists)
                    {
                        return new BadRequestResult();
                    }
                }

                // Convertir fecha apto médico a UTC
                DateTime? fechaAptoMedicoUtc = null;
                if (atletaCreateDto.FechaAptoMedico.HasValue)
                {
                    fechaAptoMedicoUtc = DateTime.SpecifyKind(atletaCreateDto.FechaAptoMedico.Value, DateTimeKind.Utc);
                }

                // Actualizar campos (FechaCreacion **no** se modifica)
                AtletaFederado.IdClub = atletaCreateDto.IdClub;
                AtletaFederado.EstadoPago = atletaCreateDto.EstadoPago;
                AtletaFederado.PerteneceSeleccion = atletaCreateDto.PerteneceSeleccion;
                AtletaFederado.Categoria = atletaCreateDto.Categoria;
                AtletaFederado.BecadoEnard = atletaCreateDto.BecadoEnard;
                AtletaFederado.BecadoSdn = atletaCreateDto.BecadoSdn;
                AtletaFederado.MontoBeca = atletaCreateDto.MontoBeca;
                AtletaFederado.PresentoAptoMedico = atletaCreateDto.PresentoAptoMedico;
                AtletaFederado.FechaAptoMedico = fechaAptoMedicoUtc;

                await _context.SaveChangesAsync();

                return new NoContentResult();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AtletaExistsAsync(id))
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

        // -------------------------------------------------
        // DELETE: Eliminar AtletaFederado
        // -------------------------------------------------
        public async Task<IActionResult> DeleteAtleta(int id)
        {
            try
            {
                var AtletaFederado = await _context.AtletasFederados
                    .Include(a => a.Participante)
                    .Include(a => a.Tutores)
                    .Include(a => a.Inscripciones)
                    .FirstOrDefaultAsync(a => a.ParticipanteId == id);

                if (AtletaFederado == null)
                {
                    return new NotFoundResult();
                }

                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // 1?? Eliminar relaciones AtletaFederado-Tutor (solo la tabla intermedia)
                    if (AtletaFederado.Tutores.Any())
                    {
                        _context.AtletasTutores.RemoveRange(AtletaFederado.Tutores);
                    }

                    // 2?? Eliminar inscripciones del AtletaFederado
                    if (AtletaFederado.Inscripciones.Any())
                    {
                        _context.Inscripciones.RemoveRange(AtletaFederado.Inscripciones);
                    }

                    // 3?? Eliminar el registro de AtletaFederado
                    _context.AtletasFederados.Remove(AtletaFederado);

                    // 4?? Verificar si la Participante tiene otros roles antes de borrarla
                    var Participante = AtletaFederado.Participante;
                    var tieneOtrosRoles = await _context.Usuarios.AnyAsync(u => u.ParticipanteId == id) ||
                                         await _context.Entrenadores.AnyAsync(e => e.ParticipanteId == id) ||
                                         await _context.DelegadosClub.AnyAsync(d => d.ParticipanteId == id) ||
                                         await _context.Tutores.AnyAsync(t => t.ParticipanteId == id);

                    // 5?? Eliminar la Participante SOLO si no tiene otros roles
                    if (!tieneOtrosRoles)
                    {
                        // Eliminar pagos asociados a la Participante (si existen)
                        var pagosPersona = await _context.PagosTransacciones
                            .Where(p => p.ParticipanteId == id)
                            .ToListAsync();

                        if (pagosPersona.Any())
                        {
                            _context.PagosTransacciones.RemoveRange(pagosPersona);
                        }

                        _context.Participantes.Remove(Participante);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new OkResult();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
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

        // -------------------------------------------------
        // Métodos auxiliares
        // -------------------------------------------------
        private async Task<bool> AtletaExistsAsync(int id)
        {
            return await _context.AtletasFederados.AnyAsync(e => e.ParticipanteId == id);
        }
    }
}


