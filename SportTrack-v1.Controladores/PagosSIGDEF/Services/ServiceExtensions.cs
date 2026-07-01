using Microsoft.Extensions.DependencyInjection;
using SportTrack_v1.Controladores.PagosSIGDEF;
using SportTrack_v1.Controladores.PagosSIGDEF.Services;

namespace SportTrack_v1.Controladores.PagosSIGDEF.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddMercadoPagoServices(this IServiceCollection services)
        {
            services.AddScoped<MercadoPagoService>();
            services.AddScoped<PaymentService>();
            return services;
        }
    }
}

