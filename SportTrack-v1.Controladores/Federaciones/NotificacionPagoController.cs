using Microsoft.AspNetCore.Mvc;
using SportTrack.AccessDatos;
using SportTrack_v1.Controladores.PagosSIGDEF.Services;
using SportTrack_v1.Entidades.Enums;
using Microsoft.EntityFrameworkCore;
using MercadoPago.Client.Payment;
using Microsoft.Extensions.Logging;

namespace SIGDEF.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificacionController : ControllerBase
    {
        private readonly SportTrackDbContext _context;
        private readonly PaymentService _paymentService;
        private readonly ILogger<NotificacionController> _logger;

        public NotificacionController(SportTrackDbContext context, PaymentService paymentService, ILogger<NotificacionController> logger)
        {
            _context = context;
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> RecibirNotificacion([FromQuery] string topic, [FromQuery] string id)
        {
            try
            {
                _logger.LogInformation($"ðŸ”” Webhook recibido: Topic={topic}, Id={id}");

                // Mercado Pago envÃ­a notificaciones de varios tipos, solo nos interesa "payment"
                if (topic == "payment")
                {
                    // 1. Consultar el estado real del pago en Mercado Pago
                    var paymentResponse = await _paymentService.GetPaymentStatusAsync("mercadopago", id);

                    if (paymentResponse.Success)
                    {
                        // 2. Obtener el detalle completo del pago para leer 'ExternalReference'
                        // Nota: GetPaymentStatusAsync devuelve un DTO genÃ©rico.
                        // Para obtener el ExternalReference necesito el objeto original o pasarlo en el DTO.
                        // SimplificaciÃ³n: Vamos a suponer que el cliente MP devuelve el external reference
                        // O ALTERNATIVA: Consultar directamente aquÃ­ con el SDK si es necesario.
                        
                        // RECOMENDACIÃ“N: Modificar PaymentResponse para incluir ExternalReference o Metadata.
                        // Pero para no romper todo ahora, voy a usar el SDK directo aquÃ­ para obtener el dato clave.
                        
                        var client = new MercadoPago.Client.Payment.PaymentClient();
                        var payment = await client.GetAsync(long.Parse(id));
                        
                        if (payment != null && !string.IsNullOrEmpty(payment.ExternalReference))
                        {
                            if (int.TryParse(payment.ExternalReference, out int idPagoDb))
                            {
                                var pagoDb = await _context.PagosTransacciones.FindAsync(idPagoDb);
                                if (pagoDb != null)
                                {
                                    // 3. Verificar estado actual para Idempotencia
                                    if (pagoDb.Estado != EstadoPagoTransaccion.Aprobado)
                                    {
                                        bool cambioEstado = false;

                                        if (payment.Status == "approved")
                                        {
                                            pagoDb.Estado = EstadoPagoTransaccion.Aprobado;
                                            pagoDb.FechaAprobacion = DateTime.UtcNow;
                                            cambioEstado = true;
                                        }
                                        else if (payment.Status == "rejected" || payment.Status == "cancelled")
                                        {
                                            // Solo pasamos a Rechazado si no estaba ya Aprobado (por seguridad)
                                            if (pagoDb.Estado != EstadoPagoTransaccion.Rechazado)
                                            {
                                                pagoDb.Estado = EstadoPagoTransaccion.Rechazado;
                                                cambioEstado = true;
                                            }
                                        }

                                        if (cambioEstado)
                                        {
                                            await _context.SaveChangesAsync();
                                            _logger.LogInformation($"âœ… BD Actualizada: Pago {idPagoDb} pasÃ³ a estado {pagoDb.Estado}");
                                        }
                                        else
                                        {
                                            _logger.LogInformation($"â„¹ï¸ Pago {idPagoDb} sin cambios de estado ({payment.Status}).");
                                        }
                                    }
                                    else
                                    {
                                        _logger.LogInformation($"â„¹ï¸ El pago {idPagoDb} ya estaba APROBADO. Se ignora actualizaciÃ³n repetida.");
                                    }
                                }
                                else
                                {
                                    _logger.LogWarning($"âš ï¸ No se encontrÃ³ transacciÃ³n con ID {idPagoDb}");
                                }
                            }
                        }
                    }
                }

                return Ok(); // Siempre responder 200 OK a Mercado Pago
            }
            catch (Exception ex)
            {
                _logger.LogError($"âŒ Error procesando webhook: {ex.Message}");
                return StatusCode(500);
            }
        }
    }
}
