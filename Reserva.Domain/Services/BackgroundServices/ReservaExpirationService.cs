using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Reserva.Common;
using Reserva.Domain.Services.Notificacion;
using Reserva.Entity;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Services.BackgroundServices
{
    /// <summary>
    /// Servicio en segundo plano que verifica y expira reservas pendientes
    /// </summary>
    public class ReservaExpirationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReservaExpirationService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(30);
        private readonly TimeSpan _warningThreshold = TimeSpan.FromHours(6);

        public ReservaExpirationService(
            IServiceProvider serviceProvider,
            ILogger<ReservaExpirationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ReservaExpirationService iniciado. Verificando cada {Minutes} minutos.", _checkInterval.TotalMinutes);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcesarReservasExpiradas(stoppingToken);
                    await NotificarReservasProximasExpirar(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al procesar reservas expiradas");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task ProcesarReservasExpiradas(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var reservaRepository = scope.ServiceProvider.GetRequiredService<IRepository<Entity.Reserva>>();
            var estadoReservaRepository = scope.ServiceProvider.GetRequiredService<IRepository<EstadoReserva>>();
            var canchaRepository = scope.ServiceProvider.GetRequiredService<IRepository<Cancha>>();
            var operadorRepository = scope.ServiceProvider.GetRequiredService<IRepository<Operador>>();
            var notificacionService = scope.ServiceProvider.GetRequiredService<INotificacionService>();

            var ahora = DateTimeOffset.Now;

            // Buscar reservas pendientes que hayan expirado
            var reservasExpiradas = await reservaRepository.FindByAsync(
                r => r.Activo
                     && r.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Pendiente
                     && r.FechaExpiracionPreReserva.HasValue
                     && r.FechaExpiracionPreReserva.Value <= ahora,
                r => r.IdEstadoReservaNavigation,
                r => r.IdCanchaNavigation,
                r => r.IdUsuarioNavigation
            );

            if (!reservasExpiradas.Any())
            {
                _logger.LogDebug("No hay reservas expiradas para procesar.");
                return;
            }

            _logger.LogInformation("Encontradas {Count} reservas expiradas para procesar", reservasExpiradas.Count());

            // Obtener el estado "Expirado"
            var estadoExpirado = await estadoReservaRepository.GetByAsNoTrackingAsync(
                e => e.Codigo == Constants.ESTADO_RESERVA.Expirado);

            if (estadoExpirado == null)
            {
                _logger.LogError("Estado 'Expirado' no encontrado en la base de datos");
                return;
            }

            foreach (var reserva in reservasExpiradas)
            {
                try
                {
                    // Actualizar estado a Expirado
                    reserva.IdEstadoReserva = estadoExpirado.IdEstadoReserva;
                    await reservaRepository.UpdateAsync(reserva);
                    await reservaRepository.SaveAsync();

                    _logger.LogInformation(
                        "Reserva {CodigoReserva} (ID: {IdReserva}) marcada como expirada",
                        reserva.CodigoReserva,
                        reserva.IdReserva);

                    // Obtener operadores de la cancha
                    var operadores = await operadorRepository.FindByAsNoTrackingAsync(
                        o => o.OperadorCancha.Any(oc => oc.IdCancha == reserva.IdCancha),
                        o => o.IdUsuarioNavigation
                    );

                    // Enviar notificación a los operadores
                    if (operadores.Any())
                    {
                        await notificacionService.NotificarReservaExpiradaAsync(
                            reserva,
                            reserva.IdCanchaNavigation,
                            operadores.ToList()
                        );

                        _logger.LogInformation(
                            "Notificación de expiración enviada para reserva {CodigoReserva}",
                            reserva.CodigoReserva);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error al procesar expiración de reserva {CodigoReserva} (ID: {IdReserva})",
                        reserva.CodigoReserva,
                        reserva.IdReserva);
                }
            }
        }

        private async Task NotificarReservasProximasExpirar(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var reservaRepository = scope.ServiceProvider.GetRequiredService<IRepository<Entity.Reserva>>();
            var operadorRepository = scope.ServiceProvider.GetRequiredService<IRepository<Operador>>();
            var notificacionService = scope.ServiceProvider.GetRequiredService<INotificacionService>();

            var ahora = DateTimeOffset.Now;
            var limiteAdvertencia = ahora.Add(_warningThreshold);

            // Buscar reservas pendientes que expiran pronto (menos de 6 horas)
            var reservasProximasExpirar = await reservaRepository.FindByAsync(
                r => r.Activo
                     && r.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Pendiente
                     && r.FechaExpiracionPreReserva.HasValue
                     && r.FechaExpiracionPreReserva.Value > ahora
                     && r.FechaExpiracionPreReserva.Value <= limiteAdvertencia
                     && !r.NotificacionAdvertenciaEnviada, // Campo adicional para evitar notificaciones duplicadas
                r => r.IdEstadoReservaNavigation,
                r => r.IdCanchaNavigation,
                r => r.IdUsuarioNavigation
            );

            if (!reservasProximasExpirar.Any())
            {
                _logger.LogDebug("No hay reservas próximas a expirar para notificar.");
                return;
            }

            _logger.LogInformation(
                "Encontradas {Count} reservas próximas a expirar (menos de {Hours} horas)",
                reservasProximasExpirar.Count(),
                _warningThreshold.TotalHours);

            foreach (var reserva in reservasProximasExpirar)
            {
                try
                {
                    // Obtener operadores de la cancha
                    var operadores = await operadorRepository.FindByAsNoTrackingAsync(
                        o => o.OperadorCancha.Any(oc => oc.IdCancha == reserva.IdCancha),
                        o => o.IdUsuarioNavigation
                    );

                    if (operadores.Any())
                    {
                        await notificacionService.NotificarReservaProximaExpirarAsync(
                            reserva,
                            reserva.IdCanchaNavigation,
                            reserva.IdUsuarioNavigation,
                            operadores.ToList()
                        );

                        // Marcar que ya se envió la notificación de advertencia
                        reserva.NotificacionAdvertenciaEnviada = true;
                        await reservaRepository.UpdateAsync(reserva);
                        await reservaRepository.SaveAsync();

                        _logger.LogInformation(
                            "Notificación de proximidad enviada para reserva {CodigoReserva} (expira en {Hours:F1} horas)",
                            reserva.CodigoReserva,
                            (reserva.FechaExpiracionPreReserva.Value - ahora).TotalHours);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error al notificar proximidad de expiración de reserva {CodigoReserva} (ID: {IdReserva})",
                        reserva.CodigoReserva,
                        reserva.IdReserva);
                }
            }
        }
    }
}
