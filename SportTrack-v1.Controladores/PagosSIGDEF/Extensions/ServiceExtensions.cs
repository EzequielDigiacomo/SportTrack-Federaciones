using Microsoft.Extensions.Configuration;
using SportTrack_v1.Controladores.PagosSIGDEF;
using SportTrack_v1.Controladores.PagosSIGDEF.Config;
using SportTrack_v1.Controladores.PagosSIGDEF.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceExtensions
{
    public static IServiceCollection AddPaymentServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configurar MercadoPago
        services.Configure<MercadoPagoSettings>(configuration.GetSection("MercadoPago"));

        // Registrar servicios
        services.AddScoped<MercadoPagoService>();
        services.AddScoped<PaymentService>();

        // Configurar HttpClient para MercadoPago si necesitas llamadas directas
        services.AddHttpClient("MercadoPago", client =>
        {
            client.BaseAddress = new Uri("https://api.mercadopago.com/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        return services;
    }
}
