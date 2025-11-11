using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Reserva.Domain.Services.BackgroundServices;
using Reserva.Domain.Services.Notificacion;
using Reserva.Domain.Services.WhatsApp;
using Microsoft.Extensions.Http;
using System.Reflection;

namespace Reserva.Domain.Extensions
{
    public static class DomainServiceCollectionExtensions
    {
        public static IServiceCollection UseDomainServices(this IServiceCollection services)
        {
            var assembly = typeof(DomainServiceCollectionExtensions).Assembly;

            // Domain Queries and Commands
            services.Scan(selector => selector
                .FromAssemblies(new List<Assembly> { assembly })
                .AddClasses(x => x.Where(c => c.Name.EndsWith("Validator")))
                .AsSelf()
                .WithScopedLifetime()
            );

            // MediatR
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);
            });

            // AutoMapper Configuration
            services.AddAutoMapper(assembly);

            // WhatsApp Service (HttpClient configurado)
            services.AddHttpClient<IWhatsAppService, WhatsAppService>();

            // Notification Service
            services.AddScoped<INotificacionService, NotificacionService>();

            // Background Services
            services.AddHostedService<ReservaExpirationService>();

            return services;
        }
    }
}
