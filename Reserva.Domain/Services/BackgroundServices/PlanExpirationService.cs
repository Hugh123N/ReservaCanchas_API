using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Reserva.Domain.Services.Notificacion;
using Reserva.Entity;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Services.BackgroundServices
{
    public class PlanExpirationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PlanExpirationService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24);

        public PlanExpirationService(
            IServiceProvider serviceProvider,
            ILogger<PlanExpirationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PlanExpirationService iniciado. Verificando cada 24 horas.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await NotificarVencimiento1Dia(stoppingToken);
                    await NotificarVencimiento5Dias(stoppingToken);
                    await ProcesarMoraYSuspension(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al procesarExpirationService");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task NotificarVencimiento1Dia(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var repos = scope.ServiceProvider.GetRequiredService<IRepository<Entity.ProveedorPlan>>();
            var notificacionService = scope.ServiceProvider.GetRequiredService<INotificacionService>();
            var proveedorRepo = scope.ServiceProvider.GetRequiredService<IRepository<Entity.Proveedor>>();
            var userRepo = scope.ServiceProvider.GetRequiredService<IRepository<Entity.AspNetUsers>>();

            var manana = DateTimeOffset.UtcNow.AddDays(1).Date;
            var finManana = manana.AddDays(1);

            var planesPorVencer = await repos.FindByAsync(x =>
                x.EsActual && x.Activo &&
                x.Estado == "ACTIVE" &&
                x.FechaFin >= manana && x.FechaFin < finManana,
                x => x.IdPlaneNavigation
            );

            var idsProveedor = planesPorVencer.Select(x => x.IdProveedor).Distinct().ToList();
            var proveedores = await proveedorRepo.FindByAsync(x => idsProveedor.Contains(x.IdProveedor));
            var proveedoresDict = proveedores.ToDictionary(x => x.IdProveedor);

            var idsUsuario = proveedores.Select(x => x.IdUsuario).Distinct().ToList();
            var usuarios = await userRepo.FindByAsync(x => idsUsuario.Contains(x.Id));
            var usuariosDict = usuarios.ToDictionary(x => x.Id);

            foreach (var pp in planesPorVencer)
            {
                if (!proveedoresDict.TryGetValue(pp.IdProveedor, out var proveedor)) continue;
                if (!usuariosDict.TryGetValue(proveedor.IdUsuario, out var usuario)) continue;
                if (string.IsNullOrEmpty(usuario.Email)) continue;

                var yaNotificado = await NotificacionYaEnviada(repos, pp.IdProveedorPlan, "VENCIMIENTO_1_DIA");
                if (yaNotificado) continue;

                await notificacionService.NotificarVencimientoPlanAsync(
                    pp,
                    pp.IdPlaneNavigation!,
                    usuario.Email
                );

                await RegistrarNotificacion(repos, pp.IdProveedorPlan, "BILLING", "VENCIMIENTO_1_DIA", usuario.Id);

                _logger.LogInformation("Notificación de vencimiento enviada para proveedor plan {Id}", pp.IdProveedorPlan);
            }
        }

        private async Task NotificarVencimiento5Dias(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var repos = scope.ServiceProvider.GetRequiredService<IRepository<Entity.ProveedorPlan>>();
            var notificacionService = scope.ServiceProvider.GetRequiredService<INotificacionService>();
            var proveedorRepo = scope.ServiceProvider.GetRequiredService<IRepository<Entity.Proveedor>>();
            var userRepo = scope.ServiceProvider.GetRequiredService<IRepository<Entity.AspNetUsers>>();

            var fechaLimite = DateTimeOffset.UtcNow.AddDays(5).Date;
            var finFechaLimite = fechaLimite.AddDays(1);

            var planesGrace = await repos.FindByAsync(x =>
                x.EsActual && x.Activo &&
                x.Estado == "GRACE" &&
                x.GracePeriodHasta >= fechaLimite && x.GracePeriodHasta < finFechaLimite,
                x => x.IdPlaneNavigation
            );

            var idsProveedor = planesGrace.Select(x => x.IdProveedor).Distinct().ToList();
            var proveedores = await proveedorRepo.FindByAsync(x => idsProveedor.Contains(x.IdProveedor));
            var proveedoresDict = proveedores.ToDictionary(x => x.IdProveedor);

            var idsUsuario = proveedores.Select(x => x.IdUsuario).Distinct().ToList();
            var usuarios = await userRepo.FindByAsync(x => idsUsuario.Contains(x.Id));
            var usuariosDict = usuarios.ToDictionary(x => x.Id);

            foreach (var pp in planesGrace)
            {
                if (!proveedoresDict.TryGetValue(pp.IdProveedor, out var proveedor)) continue;
                if (!usuariosDict.TryGetValue(proveedor.IdUsuario, out var usuario)) continue;
                if (string.IsNullOrEmpty(usuario.Email)) continue;

                var yaNotificado = await NotificacionYaEnviada(repos, pp.IdProveedorPlan, "VENCIMIENTO_5_DIAS");
                if (yaNotificado) continue;

                await notificacionService.NotificarVencimiento5DiasPlanAsync(
                    pp,
                    pp.IdPlaneNavigation!,
                    usuario.Email
                );

                await RegistrarNotificacion(repos, pp.IdProveedorPlan, "BILLING", "VENCIMIENTO_5_DIAS", usuario.Id);

                _logger.LogInformation("Notificación de suspensión en 5 días enviada para proveedor plan {Id}", pp.IdProveedorPlan);
            }
        }

        private async Task ProcesarMoraYSuspension(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var repos = scope.ServiceProvider.GetRequiredService<IRepository<Entity.ProveedorPlan>>();

            var Ahora = DateTimeOffset.UtcNow;

            var planesGraceExpirados = await repos.FindByAsync(x =>
                x.EsActual && x.Activo &&
                x.Estado == "GRACE" &&
                x.GracePeriodHasta < Ahora
            );

            foreach (var pp in planesGraceExpirados)
            {
                pp.Estado = "SUSPENDED";
                await repos.UpdateAsync(pp);
                _logger.LogInformation("Proveedor plan {Id} suspendido por mora", pp.IdProveedorPlan);
            }

            var planesSinRenovar = await repos.FindByAsync(x =>
                x.EsActual && x.Activo &&
                x.FechaFin < Ahora &&
                x.AutoRenovacion && x.Estado == "PENDING"
            );

            foreach (var pp in planesSinRenovar)
            {
                pp.Estado = "EXPIRED";
                pp.EsActual = false;
                await repos.UpdateAsync(pp);
                _logger.LogInformation("Proveedor plan {Id} marcado como expirado", pp.IdProveedorPlan);
            }

            if (planesGraceExpirados.Any() || planesSinRenovar.Any())
            {
                await repos.SaveAsync();
            }
        }

        private async Task<bool> NotificacionYaEnviada(IRepository<Entity.ProveedorPlan> repo, int idProveedorPlan, string tipo)
        {
            return false;
        }

        private async Task RegistrarNotificacion(IRepository<Entity.ProveedorPlan> repo, int idProveedorPlan, string modulo, string tipo, Guid? idUsuario)
        {
        }
    }
}
