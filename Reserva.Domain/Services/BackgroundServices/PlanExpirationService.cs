using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Reserva.Common;
using Reserva.Domain.Queries.Dbo.Notificacion;
using Reserva.Domain.Services.Culqi;
using Reserva.Domain.Services.Notificacion;
using Reserva.Dto.Dbo.Notificacion;
using MediatR;
using Reserva.Entity;
using Reserva.Repository.Abstractions.Base;
using Reserva.Domain.Commands.Dbo.Notificacion;
using System.Collections.Generic;
using System.Linq;

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
                    await ProcesarRenovacionesAutomaticas(stoppingToken);
                    await ProcesarCancelacionesAlFinPeriodo(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al procesar PlanExpirationService");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task NotificarVencimiento1Dia(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var repos = scope.ServiceProvider.GetRequiredService<IRepository<Entity.ProveedorPlan>>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var notificacionService = scope.ServiceProvider.GetRequiredService<INotificacionService>();
            var proveedorRepo = scope.ServiceProvider.GetRequiredService<IRepository<Entity.Proveedor>>();
            var userRepo = scope.ServiceProvider.GetRequiredService<IRepository<Entity.AspNetUsers>>();

            var manana = DateTimeOffset.UtcNow.AddDays(1).Date;
            var finManana = manana.AddDays(1);

            // EXCLUIR planes con CancelAtPeriodEnd=true (cancelación programada)
            var planesPorVencer = await repos.FindByAsync(x =>
                x.EsActual && x.Activo &&
                !x.CancelAtPeriodEnd &&
                x.Estado == Constants.ESTADO_PROV_PLAN.ACTIVE &&
                x.FechaFin >= manana && x.FechaFin < finManana,
                x => x.IdPlaneNavigation
            );

            // Obtener IDs masivamente
            var idsProveedorPlan = planesPorVencer.Select(x => x.IdProveedorPlan.ToString()).ToList();

            // Consulta masiva: qué IDs ya tienen notificación
            var idsFaltantes = await mediator.Send(
                new NotificacionesFaltantesQuery(
                    Constants.NOTIFICATION.BILLINGS.BILLING,
                    Constants.NOTIFICATION.BILLINGS.VENCIMIENTO_1_DIAANTES,
                    "ProveedorPlan",
                    idsProveedorPlan
                )
            );

            var idsFaltantesInt = idsFaltantes.Data!.Select(id => int.Parse(id)).ToHashSet();

            var idsProveedor = planesPorVencer.Select(x => x.IdProveedor).Distinct().ToList();
            var proveedores = await proveedorRepo.FindByAsync(x => idsProveedor.Contains(x.IdProveedor));
            var proveedoresDict = proveedores.ToDictionary(x => x.IdProveedor);

            var idsUsuario = proveedores.Select(x => x.IdUsuario).Distinct().ToList();
            var usuarios = await userRepo.FindByAsync(x => idsUsuario.Contains(x.Id));
            var usuariosDict = usuarios.ToDictionary(x => x.Id);

            var notificacionesACrear = new List<CreateNotificacionDto>();

            foreach (var pp in planesPorVencer)
            {
                if (!proveedoresDict.TryGetValue(pp.IdProveedor, out var proveedor)) continue;
                if (!usuariosDict.TryGetValue(proveedor.IdUsuario, out var usuario)) continue;
                if (string.IsNullOrEmpty(usuario.Email)) continue;

                // Match masivo: si el ID está en la lista de faltantes
                if (!idsFaltantesInt.Contains(pp.IdProveedorPlan)) continue;

                await notificacionService.NotificarVencimientoPlanAsync(
                    pp, pp.IdPlaneNavigation!, usuario.Email);

                notificacionesACrear.Add(new CreateNotificacionDto
                {
                    Modulo = Constants.NOTIFICATION.BILLINGS.BILLING,
                    Tipo = Constants.NOTIFICATION.BILLINGS.VENCIMIENTO_1_DIAANTES,
                    Canal = "EMAIL",
                    EntidadTipo = "ProveedorPlan",
                    EntidadId = pp.IdProveedorPlan.ToString(),
                    FechaEnvio = DateTimeOffset.UtcNow,
                    Intentos = 1
                });

                _logger.LogInformation("Notificación de vencimiento enviada para proveedor plan {Id}", pp.IdProveedorPlan);
            }

            if (notificacionesACrear.Any())
            {
                await mediator.Send(new CreateNotificacionesMassiveCommand(notificacionesACrear));
            }
        }

        private async Task NotificarVencimiento5Dias(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var repos = scope.ServiceProvider.GetRequiredService<IRepository<Entity.ProveedorPlan>>();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var notificacionService = scope.ServiceProvider.GetRequiredService<INotificacionService>();
            var proveedorRepo = scope.ServiceProvider.GetRequiredService<IRepository<Entity.Proveedor>>();
            var userRepo = scope.ServiceProvider.GetRequiredService<IRepository<Entity.AspNetUsers>>();

            var fechaLimite = DateTimeOffset.UtcNow.AddDays(5).Date;
            var finFechaLimite = fechaLimite.AddDays(1);

            var planesGrace = await repos.FindByAsync(x =>
                x.EsActual && x.Activo &&
                x.Estado == Constants.ESTADO_PROV_PLAN.GRACE &&
                x.GracePeriodHasta >= fechaLimite && x.GracePeriodHasta < finFechaLimite,
                x => x.IdPlaneNavigation
            );

            var idsProveedorPlan = planesGrace.Select(x => x.IdProveedorPlan.ToString()).ToList();

            // Consulta masiva: qué IDs ya tienen notificación
            var idsFaltantes = await mediator.Send(
                new NotificacionesFaltantesQuery(
                    Constants.NOTIFICATION.BILLINGS.BILLING,
                    Constants.NOTIFICATION.BILLINGS.VENCIMIENTO_5_DIAS,
                    "ProveedorPlan",
                    idsProveedorPlan
                )
            );
            var idsFaltantesInt = idsFaltantes.Data!.Select(id => int.Parse(id)).ToHashSet();

            var idsProveedor = planesGrace.Select(x => x.IdProveedor).Distinct().ToList();
            var proveedores = await proveedorRepo.FindByAsync(x => idsProveedor.Contains(x.IdProveedor));
            var proveedoresDict = proveedores.ToDictionary(x => x.IdProveedor);

            var idsUsuario = proveedores.Select(x => x.IdUsuario).Distinct().ToList();
            var usuarios = await userRepo.FindByAsync(x => idsUsuario.Contains(x.Id));
            var usuariosDict = usuarios.ToDictionary(x => x.Id);

            var notificacionesACrear = new List<CreateNotificacionDto>();

            foreach (var pp in planesGrace)
            {
                if (!proveedoresDict.TryGetValue(pp.IdProveedor, out var proveedor)) continue;
                if (!usuariosDict.TryGetValue(proveedor.IdUsuario, out var usuario)) continue;
                if (string.IsNullOrEmpty(usuario.Email)) continue;

                // Match masivo: si el ID está en la lista de faltantes
                if (!idsFaltantesInt.Contains(pp.IdProveedorPlan)) continue;

                await notificacionService.NotificarVencimiento5DiasPlanAsync(
                    pp,
                    pp.IdPlaneNavigation!,
                    usuario.Email
                );

                notificacionesACrear.Add(new CreateNotificacionDto
                {
                    Modulo = Constants.NOTIFICATION.BILLINGS.BILLING,
                    Tipo = Constants.NOTIFICATION.BILLINGS.VENCIMIENTO_5_DIAS,
                    Canal = "EMAIL",
                    EntidadTipo = "ProveedorPlan",
                    EntidadId = pp.IdProveedorPlan.ToString(),
                    FechaEnvio = DateTimeOffset.UtcNow,
                    Intentos = 1
                });

                _logger.LogInformation("Notificación de suspensión en 5 días enviada para proveedor plan {Id}", pp.IdProveedorPlan);
            }

            if (notificacionesACrear.Any())
            {
                await mediator.Send(new CreateNotificacionesMassiveCommand(notificacionesACrear));
            }
        }

        private async Task ProcesarMoraYSuspension(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var repos = scope.ServiceProvider.GetRequiredService<IRepository<Entity.ProveedorPlan>>();

            var Ahora = DateTimeOffset.UtcNow;

            var planesGraceExpirados = await repos.FindByAsync(x =>
                x.EsActual && x.Activo &&
                x.Estado == Constants.ESTADO_PROV_PLAN.GRACE &&
                x.GracePeriodHasta < Ahora
            );

            foreach (var pp in planesGraceExpirados)
            {
                pp.Estado = Constants.ESTADO_PROV_PLAN.SUSPENDED;
                await repos.UpdateAsync(pp);
                _logger.LogInformation("Proveedor plan {Id} suspendido por mora", pp.IdProveedorPlan);
            }

            if (planesGraceExpirados.Any())
            {
                await repos.SaveAsync();
            }
        }

        private async Task ProcesarRenovacionesAutomaticas(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var repos = scope.ServiceProvider.GetRequiredService<IRepository<Entity.ProveedorPlan>>();
            var culqiService = scope.ServiceProvider.GetRequiredService<ICulqiService>();

            var Ahora = DateTimeOffset.UtcNow;

            // Buscar planes con AutoRenovacion=true y cuya FechaProximoCobro ya pasó
            // EXCLUIR planes con CancelAtPeriodEnd=true (cancelación programada)
            var planesARenovar = await repos.FindByAsync(x =>
                x.EsActual && x.Activo &&
                !x.CancelAtPeriodEnd &&
                x.AutoRenovacion &&
                x.Estado == Constants.ESTADO_PROV_PLAN.ACTIVE &&
                x.FechaProximoCobro.HasValue &&
                x.FechaProximoCobro.Value <= Ahora
            );

            foreach (var pp in planesARenovar)
            {
                try
                {
                    // Verificar estado de la suscripción en Culqi
                    if (!string.IsNullOrEmpty(pp.CulqiSubscriptionId))
                    {
                        var subscription = await culqiService.GetSubscriptionAsync(pp.CulqiSubscriptionId);
                        if (subscription != null && subscription.Status == Constants.CULQI_SUBSCRIPTION_STATUS.ACTIVE)
                        {
                            // La renovación ya se procesó en Culqi, solo actualizamos fechas
                            pp.FechaFin = DateTimeOffset.FromUnixTimeSeconds(subscription.NextBillingDate ?? 0);
                            pp.FechaProximoCobro = pp.FechaFin;
                            await repos.UpdateAsync(pp);
                            _logger.LogInformation("Renovación automática procesada para ProveedorPlan {Id}", pp.IdProveedorPlan);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al procesar renovación automática para ProveedorPlan {Id}", pp.IdProveedorPlan);
                }
            }

            if (planesARenovar.Any())
            {
                await repos.SaveAsync();
            }
        }

        private async Task ProcesarCancelacionesAlFinPeriodo(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var repos = scope.ServiceProvider.GetRequiredService<IRepository<Entity.ProveedorPlan>>();

            var Ahora = DateTimeOffset.UtcNow;

            // Buscar planes con CancelAtPeriodEnd=true y cuya FechaFin ya pasó
            // Estos planes deben cambiar a CANCELLED (no GRACE, no notificaciones)
            var planesACancelar = await repos.FindByAsync(x =>
                x.EsActual && x.Activo &&
                x.CancelAtPeriodEnd &&
                x.Estado == Constants.ESTADO_PROV_PLAN.ACTIVE &&
                x.FechaFin < Ahora
            );

            foreach (var pp in planesACancelar)
            {
                pp.Estado = Constants.ESTADO_PROV_PLAN.CANCELLED;
                pp.EsActual = false;
                await repos.UpdateAsync(pp);
                _logger.LogInformation("Proveedor plan {Id} cancelado al finalizar período (CancelAtPeriodEnd)", pp.IdProveedorPlan);
            }

            if (planesACancelar.Any())
            {
                await repos.SaveAsync();
            }
        }
    }
}
