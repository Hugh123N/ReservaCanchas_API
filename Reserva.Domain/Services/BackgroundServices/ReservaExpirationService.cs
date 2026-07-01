using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Reserva.Common;
using Reserva.Domain.Helpers;
using Reserva.Domain.Services.Notificacion;
using Reserva.Entity;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Services.BackgroundServices
{
    /// <summary>
    /// Servicio en segundo plano que verifica y expira reservas pendientes,
    /// envía recordatorios 1 hora antes y advertencias 6 horas antes de expirar
    /// </summary>
    public class ReservaExpirationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReservaExpirationService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(30);
        private readonly TimeSpan _warningThreshold = TimeSpan.FromHours(6);
        private readonly TimeSpan _reminderThreshold = TimeSpan.FromHours(1);

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
                    await EnviarRecordatoriosReservasProximas(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al procesar reservas");
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

            var ahora = DateTimeOffset.UtcNow;

            // Buscar reservas pendientes que hayan expirado
            var reservasExpiradas = await reservaRepository.FindByAsync(
                r => r.Activo
                     && r.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Pendiente
                     && r.FechaExpiracionPreReserva.HasValue
                     && r.FechaExpiracionPreReserva.Value <= ahora,
                r => r.IdEstadoReservaNavigation,
                r => r.IdCanchaNavigation,
                r => r.IdClienteNavigation
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

            var ahora = DateTimeOffset.UtcNow;
            var limiteAdvertencia = ahora.Add(_warningThreshold);

            // Buscar reservas pendientes que expiran pronto (menos de 6 horas)
            var reservasProximasExpirar = await reservaRepository.FindByAsync(
                r => r.Activo
                     && r.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Pendiente
                     && r.FechaExpiracionPreReserva.HasValue
                     && r.FechaExpiracionPreReserva.Value > ahora
                     && r.FechaExpiracionPreReserva.Value <= limiteAdvertencia
                     && !(r.NotificacionAdvertenciaEnviada ?? false), 
                r => r.IdEstadoReservaNavigation,
                r => r.IdCanchaNavigation,
                r => r.IdClienteNavigation
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
                    var operadores = await operadorRepository.FindByAsNoTrackingAsync(
                        o => o.OperadorCancha.Any(oc => oc.IdCancha == reserva.IdCancha),
                        o => o.IdUsuarioNavigation
                    );

                    if (operadores.Any())
                    {
                        await notificacionService.NotificarReservaProximaExpirarAsync(
                            reserva,
                            reserva.IdCanchaNavigation,
                            reserva.IdClienteNavigation,
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

        private async Task EnviarRecordatoriosReservasProximas(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var reservaRepository = scope.ServiceProvider.GetRequiredService<IRepository<Entity.Reserva>>();
            var detalleReservaRepository = scope.ServiceProvider.GetRequiredService<IRepository<DetalleReserva>>();
            var notificacionService = scope.ServiceProvider.GetRequiredService<INotificacionService>();

            var ahora = DateTimeOffset.UtcNow;

            // Buscar reservas CONFIRMADAS que empiezan en menos de 1 hora
            // y que NO hayan sido notificadas aún (recordatorioEnviado = false)
            var reservasProximas = await reservaRepository.FindByAsync(
                r => r.Activo
                     && r.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Confirmado
                     && r.FechaReserva.Date == ahora.Date // Solo reservas de hoy
                     && !(r.RecordatorioEnviado ?? false),
                r => r.IdEstadoReservaNavigation,
                r => r.IdCanchaNavigation,
                r => r.IdClienteNavigation
            );

            if (!reservasProximas.Any())
            {
                _logger.LogDebug("No hay reservas confirmadas para enviar recordatorios.");
                return;
            }

            _logger.LogInformation("Verificando {Count} reservas confirmadas para recordatorios", reservasProximas.Count());

            foreach (var reserva in reservasProximas)
            {
                try
                {
                    var detallesConHorarios = await detalleReservaRepository.FindByAsNoTrackingAsync(
                        d => d.IdReserva == reserva.IdReserva && d.Activo,
                        d => d.IdHorarioCanchaNavigation!.IdHoraInicioNavigation!,
                        d => d.IdHorarioCanchaNavigation!.IdHoraFinNavigation!
                    );

                    if (!detallesConHorarios.Any())
                        continue;

                    // Obtener primer horario para verificar si enviar recordatorio
                    var primerDetalle = detallesConHorarios
                        .Where(d => d.IdHorarioCanchaNavigation?.IdHoraInicioNavigation != null)
                        .OrderBy(d => d.IdHorarioCanchaNavigation!.IdHoraInicioNavigation!.Hora1)
                        .FirstOrDefault();

                    if (primerDetalle == null)
                        continue;

                    // Construir DateTime completo de la reserva
                    var fechaHoraReserva = reserva.FechaReserva.Date
                        .Add(primerDetalle.IdHorarioCanchaNavigation!.IdHoraInicioNavigation!.Hora1.ToTimeSpan());

                    var tiempoHastaReserva = fechaHoraReserva - ahora;

                    // Si falta menos de 1 hora (pero más de 0) → enviar recordatorio
                    if (tiempoHastaReserva.TotalMinutes > 0 && tiempoHastaReserva.TotalMinutes <= 60)
                    {
                        // Extraer horarios y formatear
                        var horariosLista = detallesConHorarios
                            .Where(d => d.IdHorarioCanchaNavigation?.IdHoraInicioNavigation != null
                                     && d.IdHorarioCanchaNavigation?.IdHoraFinNavigation != null)
                            .Select(d => {
                                var horaInicio = d.IdHorarioCanchaNavigation!.IdHoraInicioNavigation!.Hora1;
                                var horaFin = d.IdHorarioCanchaNavigation!.IdHoraFinNavigation!.Hora1;
                                return (inicio: horaInicio, fin: horaFin);
                            })
                            .OrderBy(h => h.inicio)
                            .ToList();

                        var horariosFormateado = HorarioHelper.FormatearHorariosConsecutivos(horariosLista);

                        await notificacionService.NotificarRecordatorioReservaAsync(
                            reserva,
                            reserva.IdCanchaNavigation,
                            reserva.IdClienteNavigation,
                            horariosFormateado
                        );

                        // Marcar como notificada
                        reserva.RecordatorioEnviado = true;
                        await reservaRepository.UpdateAsync(reserva);
                        await reservaRepository.SaveAsync();

                        _logger.LogInformation(
                            "Recordatorio enviado para reserva {CodigoReserva} (empieza en {Minutos} minutos)",
                            reserva.CodigoReserva,
                            tiempoHastaReserva.TotalMinutes);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error al enviar recordatorio de reserva {CodigoReserva} (ID: {IdReserva})",
                        reserva.CodigoReserva,
                        reserva.IdReserva);
                }
            }
        }
    }
}
