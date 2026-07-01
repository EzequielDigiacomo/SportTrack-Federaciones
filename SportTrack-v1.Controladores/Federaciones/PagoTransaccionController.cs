using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportTrack_v1.Controladores.PagosSIGDEF.Models.Dtos;
using SportTrack.AccessDatos;
using SIGDEF.DTOs;
using SportTrack_v1.Entidades.Entidades;
using SportTrack_v1.Entidades.DTOs.Club;
using SportTrack_v1.Entidades.DTOs.PagoTransaccion;
using SportTrack_v1.Entidades.DTOs.Participante;
using SportTrack_v1.Entidades.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIGDEF.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagoTransaccionController : ControllerBase
    {
        private readonly SportTrackDbContext _context;
        private readonly SportTrack_v1.Controladores.PagosSIGDEF.Services.PaymentService _paymentService;

        public PagoTransaccionController(SportTrackDbContext context, SportTrack_v1.Controladores.PagosSIGDEF.Services.PaymentService paymentService)
        {
            _context = context;
            _paymentService = paymentService;
        }

        // POST: api/PagoTransaccion/preferencia
        [HttpPost("preferencia")]
        public async Task<ActionResult<PaymentResponse>> CrearPreferenciaPago(PagoTransaccionCreateDto pagoDto)
        {
            try
            {
                // 1. Validar Participante y Club
                var Participante = await _context.Participantes.FindAsync(pagoDto.ParticipanteId);
                if (Participante == null) return BadRequest("La Participante especificada no existe");

                var clubExists = await _context.Clubes.AnyAsync(c => c.IdClub == pagoDto.IdClub);
                if (!clubExists) return BadRequest("El club especificado no existe");

                // 2. Crear primero el registro en BD (Pendiente) para tener un ID único
                var pagoTransaccion = new PagoTransaccion
                {
                    Concepto = pagoDto.Concepto,
                    Monto = pagoDto.Monto,
                    Estado = EstadoPagoTransaccion.Pendiente,
                    FechaCreacion = DateTime.UtcNow,
                    ParticipanteId = pagoDto.ParticipanteId,
                    IdClub = pagoDto.IdClub,
                    IdMercadoPago = "PENDING" // Se actualizará al generar el link
                };

                _context.PagosTransacciones.Add(pagoTransaccion);
                await _context.SaveChangesAsync(); // Obtenemos pagoTransaccion.IdPago

                // 3. Crear Request para MercadoPago usando el IdPago como Referencia
                var paymentRequest = new PaymentRequest
                {
                    Amount = pagoDto.Monto,
                    Description = pagoDto.Concepto,
                    CustomerEmail = Participante.Email ?? "email@default.com",
                    CustomerName = $"{Participante.Nombre} {Participante.Apellido}",
                    Gateway = "MercadoPago",
                    Metadata = new Dictionary<string, string>
                    {
                        { "IdClub", pagoDto.IdClub.ToString() },
                        { "ParticipanteId", pagoDto.ParticipanteId.ToString() },
                        { "ReferenceId", pagoTransaccion.IdPago.ToString() } // VINCULACIÓN CLAVE
                    }
                };

                // 4. Llamar al servicio de pagos
                var result = await _paymentService.ProcessPaymentAsync(paymentRequest);

                if (result.Success)
                {
                    // 5. Actualizar el registro con el ID de la preferencia
                    pagoTransaccion.IdMercadoPago = result.PaymentId;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // Si falla MP, podríamos borrar el registro o marcarlo como error
                    _context.PagosTransacciones.Remove(pagoTransaccion);
                    await _context.SaveChangesAsync();
                    return StatusCode(500, $"Error al generar preferencia en MP: {result.ErrorMessage}");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generando preferencia de pago: {ex.Message}");
            }
        }

        // GET: api/PagoTransaccion
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PagoTransaccionDto>>> GetPagosTransaccion()
        {
            try
            {
                var pagos = await _context.PagosTransacciones
                    .Include(p => p.Participante)
                    .Include(p => p.Club)
                    .Select(p => new PagoTransaccionDto
                    {
                        IdPago = p.IdPago,
                        Concepto = p.Concepto,
                        Monto = p.Monto,
                        Estado = p.Estado,
                        FechaCreacion = p.FechaCreacion,
                        FechaAprobacion = p.FechaAprobacion,
                        ParticipanteId = p.ParticipanteId,
                        IdClub = p.IdClub,
                        IdMercadoPago = p.IdMercadoPago,
                        NombrePersona = p.Participante.Nombre + " " + p.Participante.Apellido,
                        NombreClub = p.Club.Nombre,
                        EstadoDescripcion = GetEstadoDescripcion(p.Estado)
                    })
                    .ToListAsync();

                return Ok(pagos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/PagoTransaccion/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PagoTransaccionDetailDto>> GetPagoTransaccion(int id)
        {
            try
            {
                var pago = await _context.PagosTransacciones
                    .Include(p => p.Participante)
                    .Include(p => p.Club)
                    .Where(p => p.IdPago == id)
                    .Select(p => new PagoTransaccionDetailDto
                    {
                        IdPago = p.IdPago,
                        Concepto = p.Concepto,
                        Monto = p.Monto,
                        Estado = p.Estado,
                        FechaCreacion = p.FechaCreacion,
                        FechaAprobacion = p.FechaAprobacion,
                        ParticipanteId = p.ParticipanteId,
                        IdClub = p.IdClub,
                        IdMercadoPago = p.IdMercadoPago,
                        Participante = new PersonaDto
                        {
                            ParticipanteId = p.Participante.ParticipanteId,
                            Nombre = p.Participante.Nombre,
                            Apellido = p.Participante.Apellido,
                            Documento = p.Participante.Dni,
                            FechaNacimiento = p.Participante.FechaNacimiento,
                            Email = p.Participante.Email,
                            Telefono = p.Participante.Telefono,
                            Direccion = p.Participante.Direccion
                        },
                        Club = new ClubDto
                        {
                            IdClub = p.Club.IdClub,
                            Nombre = p.Club.Nombre,
                            Direccion = p.Club.Direccion,
                            Telefono = p.Club.Telefono,
                            Siglas = p.Club.Siglas
                        }
                    })
                    .FirstOrDefaultAsync();

                if (pago == null)
                {
                    return NotFound($"PagoTransaccion con ID {id} no encontrado");
                }

                return Ok(pago);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/PagoTransaccion/Participante/5
        [HttpGet("Participante/{ParticipanteId}")]
        public async Task<ActionResult<IEnumerable<PagoTransaccionDto>>> GetPagosPorPersona(int ParticipanteId)
        {
            try
            {
                var pagos = await _context.PagosTransacciones
                    .Include(p => p.Participante)
                    .Include(p => p.Club)
                    .Where(p => p.ParticipanteId == ParticipanteId)
                    .Select(p => new PagoTransaccionDto
                    {
                        IdPago = p.IdPago,
                        Concepto = p.Concepto,
                        Monto = p.Monto,
                        Estado = p.Estado,
                        FechaCreacion = p.FechaCreacion,
                        FechaAprobacion = p.FechaAprobacion,
                        ParticipanteId = p.ParticipanteId,
                        IdClub = p.IdClub,
                        IdMercadoPago = p.IdMercadoPago,
                        NombrePersona = p.Participante.Nombre + " " + p.Participante.Apellido,
                        NombreClub = p.Club.Nombre,
                        EstadoDescripcion = GetEstadoDescripcion(p.Estado)
                    })
                    .ToListAsync();

                return Ok(pagos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/PagoTransaccion/club/5
        [HttpGet("club/{idClub}")]
        public async Task<ActionResult<IEnumerable<PagoTransaccionDto>>> GetPagosPorClub(int idClub)
        {
            try
            {
                var pagos = await _context.PagosTransacciones
                    .Include(p => p.Participante)
                    .Include(p => p.Club)
                    .Where(p => p.IdClub == idClub)
                    .Select(p => new PagoTransaccionDto
                    {
                        IdPago = p.IdPago,
                        Concepto = p.Concepto,
                        Monto = p.Monto,
                        Estado = p.Estado,
                        FechaCreacion = p.FechaCreacion,
                        FechaAprobacion = p.FechaAprobacion,
                        ParticipanteId = p.ParticipanteId,
                        IdClub = p.IdClub,
                        IdMercadoPago = p.IdMercadoPago,
                        NombrePersona = p.Participante.Nombre + " " + p.Participante.Apellido,
                        NombreClub = p.Club.Nombre,
                        EstadoDescripcion = GetEstadoDescripcion(p.Estado)
                    })
                    .ToListAsync();

                return Ok(pagos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/PagoTransaccion/estado/pendiente
        [HttpGet("estado/{estado}")]
        public async Task<ActionResult<IEnumerable<PagoTransaccionDto>>> GetPagosPorEstado(EstadoPagoTransaccion estado)
        {
            try
            {
                var pagos = await _context.PagosTransacciones
                    .Include(p => p.Participante)
                    .Include(p => p.Club)
                    .Where(p => p.Estado == estado)
                    .Select(p => new PagoTransaccionDto
                    {
                        IdPago = p.IdPago,
                        Concepto = p.Concepto,
                        Monto = p.Monto,
                        Estado = p.Estado,
                        FechaCreacion = p.FechaCreacion,
                        FechaAprobacion = p.FechaAprobacion,
                        ParticipanteId = p.ParticipanteId,
                        IdClub = p.IdClub,
                        IdMercadoPago = p.IdMercadoPago,
                        NombrePersona = p.Participante.Nombre + " " + p.Participante.Apellido,
                        NombreClub = p.Club.Nombre,
                        EstadoDescripcion = GetEstadoDescripcion(p.Estado)
                    })
                    .ToListAsync();

                return Ok(pagos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // POST: api/PagoTransaccion
        [HttpPost]
        public async Task<ActionResult<PagoTransaccionDto>> PostPagoTransaccion(PagoTransaccionCreateDto pagoTransaccionCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // ?? Validar que la Participante existe
                var personaExists = await _context.Participantes.AnyAsync(p => p.ParticipanteId == pagoTransaccionCreateDto.ParticipanteId);
                if (!personaExists)
                {
                    return BadRequest("La Participante especificada no existe");
                }

                // ?? Validar que el club existe
                var clubExists = await _context.Clubes.AnyAsync(c => c.IdClub == pagoTransaccionCreateDto.IdClub);
                if (!clubExists)
                {
                    return BadRequest("El club especificado no existe");
                }

                var pagoTransaccion = new PagoTransaccion
                {
                    Concepto = pagoTransaccionCreateDto.Concepto,
                    Monto = pagoTransaccionCreateDto.Monto,
                    Estado = pagoTransaccionCreateDto.Estado,
                    FechaCreacion = DateTime.UtcNow,
                    FechaAprobacion = pagoTransaccionCreateDto.Estado == EstadoPagoTransaccion.Aprobado ? DateTime.UtcNow : null,
                    ParticipanteId = pagoTransaccionCreateDto.ParticipanteId,
                    IdClub = pagoTransaccionCreateDto.IdClub,
                    IdMercadoPago = pagoTransaccionCreateDto.IdMercadoPago ?? string.Empty
                };

                _context.PagosTransacciones.Add(pagoTransaccion);
                await _context.SaveChangesAsync();

                // ?? Cargar datos relacionados para la respuesta
                await _context.Entry(pagoTransaccion)
                    .Reference(p => p.Participante)
                    .LoadAsync();
                await _context.Entry(pagoTransaccion)
                    .Reference(p => p.Club)
                    .LoadAsync();

                var pagoTransaccionDto = new PagoTransaccionDto
                {
                    IdPago = pagoTransaccion.IdPago,
                    Concepto = pagoTransaccion.Concepto,
                    Monto = pagoTransaccion.Monto,
                    Estado = pagoTransaccion.Estado,
                    FechaCreacion = pagoTransaccion.FechaCreacion,
                    FechaAprobacion = pagoTransaccion.FechaAprobacion,
                    ParticipanteId = pagoTransaccion.ParticipanteId,
                    IdClub = pagoTransaccion.IdClub,
                    IdMercadoPago = pagoTransaccion.IdMercadoPago,
                    NombrePersona = pagoTransaccion.Participante.Nombre + " " + pagoTransaccion.Participante.Apellido,
                    NombreClub = pagoTransaccion.Club.Nombre,
                    EstadoDescripcion = GetEstadoDescripcion(pagoTransaccion.Estado)
                };

                // ?? RETORNA 201 CREATED
                return CreatedAtAction(nameof(GetPagoTransaccion), new { id = pagoTransaccion.IdPago }, pagoTransaccionDto);
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Error de base de datos: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // PUT: api/PagoTransaccion/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPagoTransaccion(int id, PagoTransaccionCreateDto pagoTransaccionCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var pagoTransaccion = await _context.PagosTransacciones.FindAsync(id);
                if (pagoTransaccion == null)
                {
                    return NotFound($"PagoTransaccion con ID {id} no encontrado");
                }

                // ?? Validar que la Participante existe
                var personaExists = await _context.Participantes.AnyAsync(p => p.ParticipanteId == pagoTransaccionCreateDto.ParticipanteId);
                if (!personaExists)
                {
                    return BadRequest("La Participante especificada no existe");
                }

                // ?? Validar que el club existe
                var clubExists = await _context.Clubes.AnyAsync(c => c.IdClub == pagoTransaccionCreateDto.IdClub);
                if (!clubExists)
                {
                    return BadRequest("El club especificado no existe");
                }

                // Actualizar propiedades
                pagoTransaccion.Concepto = pagoTransaccionCreateDto.Concepto;
                pagoTransaccion.Monto = pagoTransaccionCreateDto.Monto;
                pagoTransaccion.Estado = pagoTransaccionCreateDto.Estado;
                pagoTransaccion.ParticipanteId = pagoTransaccionCreateDto.ParticipanteId;
                pagoTransaccion.IdClub = pagoTransaccionCreateDto.IdClub;
                pagoTransaccion.IdMercadoPago = pagoTransaccionCreateDto.IdMercadoPago ?? string.Empty;

                // ?? Si el estado cambia a Aprobado y no tiene fecha de aprobación, establecerla
                if (pagoTransaccionCreateDto.Estado == EstadoPagoTransaccion.Aprobado && !pagoTransaccion.FechaAprobacion.HasValue)
                {
                    pagoTransaccion.FechaAprobacion = DateTime.UtcNow;
                }
                // ?? Si el estado cambia de Aprobado a otro estado, limpiar la fecha de aprobación
                else if (pagoTransaccionCreateDto.Estado != EstadoPagoTransaccion.Aprobado && pagoTransaccion.FechaAprobacion.HasValue)
                {
                    pagoTransaccion.FechaAprobacion = null;
                }

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PagoTransaccionExists(id))
                {
                    return NotFound($"PagoTransaccion con ID {id} no existe");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // PATCH: api/PagoTransaccion/5/estado
        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> UpdateEstadoPago(int id, [FromBody] EstadoPagoTransaccion nuevoEstado)
        {
            try
            {
                var pagoTransaccion = await _context.PagosTransacciones.FindAsync(id);
                if (pagoTransaccion == null)
                {
                    return NotFound($"PagoTransaccion con ID {id} no encontrado");
                }

                pagoTransaccion.Estado = nuevoEstado;

                // ?? Actualizar fecha de aprobación si corresponde
                if (nuevoEstado == EstadoPagoTransaccion.Aprobado && !pagoTransaccion.FechaAprobacion.HasValue)
                {
                    pagoTransaccion.FechaAprobacion = DateTime.UtcNow;
                }
                else if (nuevoEstado != EstadoPagoTransaccion.Aprobado && pagoTransaccion.FechaAprobacion.HasValue)
                {
                    pagoTransaccion.FechaAprobacion = null;
                }

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // DELETE: api/PagoTransaccion/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePagoTransaccion(int id)
        {
            try
            {
                var pagoTransaccion = await _context.PagosTransacciones.FindAsync(id);
                if (pagoTransaccion == null)
                {
                    return NotFound($"PagoTransaccion con ID {id} no encontrado");
                }

                _context.PagosTransacciones.Remove(pagoTransaccion);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, $"Error al eliminar el pago: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/PagoTransaccion/estadisticas
        [HttpGet("estadisticas")]
        public async Task<ActionResult<object>> GetEstadisticasPagos()
        {
            try
            {
                var estadisticas = new
                {
                    TotalPagos = await _context.PagosTransacciones.CountAsync(),
                    TotalMonto = await _context.PagosTransacciones.SumAsync(p => p.Monto),
                    PagosPendientes = await _context.PagosTransacciones.CountAsync(p => p.Estado == EstadoPagoTransaccion.Pendiente),
                    PagosAprobados = await _context.PagosTransacciones.CountAsync(p => p.Estado == EstadoPagoTransaccion.Aprobado),
                    PagosRechazados = await _context.PagosTransacciones.CountAsync(p => p.Estado == EstadoPagoTransaccion.Rechazado),
                    MontoAprobado = await _context.PagosTransacciones
                        .Where(p => p.Estado == EstadoPagoTransaccion.Aprobado)
                        .SumAsync(p => p.Monto)
                };

                return Ok(estadisticas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        private bool PagoTransaccionExists(int id)
        {
            return _context.PagosTransacciones.Any(e => e.IdPago == id);
        }

        private static string GetEstadoDescripcion(EstadoPagoTransaccion estado)
        {
            return estado switch
            {
                EstadoPagoTransaccion.Pendiente => "Pendiente",
                EstadoPagoTransaccion.Aprobado => "Aprobado",
                EstadoPagoTransaccion.Rechazado => "Rechazado",
                _ => "Desconocido"
            };
        }
    }
}
