// ?? Services/MercadoPagoService.cs
using MercadoPago.Client.Common;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Preference;
using Microsoft.Extensions.Options;
using SportTrack_v1.Controladores.PagosSIGDEF.Models.Dtos;
using SportTrack_v1.Controladores.PagosSIGDEF.Config;

namespace SportTrack_v1.Controladores.PagosSIGDEF;

public class MercadoPagoService
{
    private readonly string _accessToken;
    private readonly string _notificationUrl;

    public MercadoPagoService(IOptions<MercadoPagoSettings> config)
    {
        _accessToken = config.Value.AccessToken;
        _notificationUrl = config.Value.NotificationUrl;

        // Configurar SDK de MercadoPago
        MercadoPagoConfig.AccessToken = _accessToken;
    }

    public async Task<PaymentResponse> CreatePreferenceAsync(PaymentRequest request)
    {
        try
        {
            var client = new PreferenceClient();

            // 1. Crear items de la preferencia
            var items = new List<PreferenceItemRequest>
            {
                new PreferenceItemRequest
                {
                    Title = request.Description,
                    Quantity = 1,
                    CurrencyId = "ARS",
                    UnitPrice = request.Amount
                }
            };

            // 2. Configurar pagador
            var payer = new PreferencePayerRequest
            {
                Email = request.CustomerEmail,
                Name = request.CustomerName,
                Surname = "", // Opcional si viene en el nombre
            };

            // 3. Crear la solicitud de preferencia
            var preferenceRequest = new PreferenceRequest
            {
                Items = items,
                Payer = payer,
                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = "https://tu-dominio.com/pagos/exito", // TODO: Configurar URL real
                    Failure = "https://tu-dominio.com/pagos/fallo",
                    Pending = "https://tu-dominio.com/pagos/pendiente"
                },
                AutoReturn = "approved",
                ExternalReference = request.Metadata.ContainsKey("ReferenceId") ? request.Metadata["ReferenceId"] : Guid.NewGuid().ToString(),
                StatementDescriptor = "SIGDEF PAGO",
                NotificationUrl = !string.IsNullOrEmpty(_notificationUrl) ? _notificationUrl : null
            };

            // 4. Generar la preferencia
            Preference preference = await client.CreateAsync(preferenceRequest);

            return new PaymentResponse
            {
                Success = true,
                PaymentId = preference.Id, // ID de la preferencia
                Status = "created",
                PaymentUrl = preference.InitPoint, // LINK DE PAGO (Aquí está la magia para el QR)
                SandboxPaymentUrl = preference.SandboxInitPoint
            };
        }
        catch (Exception ex)
        {
            return new PaymentResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<PaymentResponse> GetPaymentStatusAsync(string paymentId)
    {
        // Nota: Para verificar estado de PAGO real se usa PaymentClient, 
        // pero 'paymentId' aquí podría ser el ID de la Preferencia o del Pago.
        // Por ahora lo dejamos igual para consultar Pagos.
        try
        {
            var client = new MercadoPago.Client.Payment.PaymentClient();
            var payment = await client.GetAsync(long.Parse(paymentId));

            return new PaymentResponse
            {
                Success = true,
                PaymentId = payment.Id.ToString(),
                Status = payment.Status,
                PaymentUrl = payment.PointOfInteraction?.TransactionData?.TicketUrl 
                             ?? $"https://www.mercadopago.com.ar/checkout/v1/payment/{payment.Id}"
            };
        }
        catch (Exception ex)
        {
            return new PaymentResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
