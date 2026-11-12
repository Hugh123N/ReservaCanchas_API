using AutoMapper;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Reserva;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using Reserva.Repository.Utils;
using Reserva.Domain.Services.Pago;
using Reserva.Domain.Helpers;

namespace Reserva.Domain.Commands.Dbo.Reserva
{
    public class UpdateReservaCommandHandler : CommandHandlerBase<UpdateReservaCommand, GetReservaDto>
    {
        private readonly IRepository<Entity.Reserva> _reservaRepository;
        private readonly IRepository<Entity.DetalleReserva> _detalleReservaRepository;
        private readonly IRepository<Entity.HorarioCancha> _horarioCanchaRepository;
        private readonly IRepository<Entity.EstadoPago> _estadoPagoRepository;
        private readonly PagoReservaAccessService _access;

        public UpdateReservaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateReservaCommandValidator validator,
            IRepository<Entity.Reserva> reservaRepository,
            IRepository<Entity.DetalleReserva> detalleReservaRepository,
            IRepository<Entity.HorarioCancha> horarioCanchaRepository,
            IRepository<Entity.EstadoPago> estadoPagoRepository,
            PagoReservaAccessService access
        ) : base(unitOfWork, mapper, validator)
        {
            _reservaRepository = reservaRepository;
            _detalleReservaRepository = detalleReservaRepository;
            _horarioCanchaRepository = horarioCanchaRepository;
            _estadoPagoRepository = estadoPagoRepository;
            _access = access;
        }

        public override async Task<ResponseDto<GetReservaDto>> HandleCommand(UpdateReservaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetReservaDto>();
            var dto = request.UpdateDto;

            try
            {
                var reserva = await _reservaRepository.GetByAsync(x => x.IdReserva == dto.IdReserva && x.Activo,
                    x => x.DetalleReserva.Where(x => x.Activo),
                    x => x.Pago.Where(x => x.Activo),
                    x => x.IdCanchaNavigation.IdProveedorNavigation.ConfiguracionProveedor!,
                    x => x.IdEstadoReservaNavigation,
                    x => x.IdClienteNavigation!,
                    x => x.IdTipoDeporteNavigation!,
                    x => x.IdOperadorConfirmoNavigation!
                );

                if (reserva == null)
                {
                    response.AddErrorResult("Reserva no encontrada");
                    return response;
                }

                if (!await _access.PuedeAccederAsync(reserva) || !_access.PuedeOperarPorFecha(reserva))
                {
                    response.AddErrorResult("No tienes permiso para editar esta reserva.");
                    return response;
                }
                var pago = reserva.Pago.FirstOrDefault();
                if (pago == null || dto.RevisionPago != pago.Revision)
                {
                    response.AddErrorResult("El pago cambió o falta su versión. Recarga la reserva antes de editar.");
                    return response;
                }
                if (dto.IdCancha != reserva.IdCancha || dto.FechaReserva.Date != reserva.FechaReserva.Date ||
                    dto.PagoReserva == null || dto.PagoReserva.Count > 1 || dto.PagoReserva.Any(p => p.IdPago != pago.IdPago || p.MontoAdelanto != pago.MontoAdelanto))
                {
                    response.AddErrorResult("La edición no permite cambiar la cancha, fecha ni el acumulado cobrado. Usa Registrar cobro o Corregir registro.");
                    return response;
                }
                if (reserva.IdEstadoReservaNavigation.Codigo != Constants.ESTADO_RESERVA.Pendiente &&
                    reserva.IdEstadoReservaNavigation.Codigo != Constants.ESTADO_RESERVA.Confirmado)
                {
                    response.AddErrorResult("La reserva no está vigente para editarla.");
                    return response;
                }

                reserva.Observaciones = dto.Observaciones;
                var horarios = new List<Entity.HorarioCancha>();

                if (dto.Horarios != null && dto.Horarios.Any())
                {
                    foreach (var bloque in dto.Horarios)
                    {
                        var dia = (int)bloque.Fecha.DayOfWeek;
                        if (dia == 0) dia = 7;
                        if (bloque.Fecha.Date != reserva.FechaReserva.Date)
                        {
                            response.AddErrorResult("Los bloques deben pertenecer a la fecha de la reserva.");
                            return response;
                        }
                        var horariosBloque = (await _horarioCanchaRepository.FindByAsync(h => h.Activo && h.IdCancha == reserva.IdCancha &&
                            h.IdDiaSemana == dia && h.IdHorarioCancha >= bloque.IdHorarioCanchaInicio && h.IdHorarioCancha <= bloque.IdHorarioCanchaFin,
                            h => h.IdHoraInicioNavigation, h => h.IdHoraFinNavigation!)).ToList();
                        if (horariosBloque.Count == 0 || !horariosBloque.Any(h => h.IdHorarioCancha == bloque.IdHorarioCanchaInicio) ||
                            !horariosBloque.Any(h => h.IdHorarioCancha == bloque.IdHorarioCanchaFin))
                        {
                            response.AddErrorResult("No se encontraron horarios configurados para el bloque seleccionado.");
                            return response;
                        }
                        horarios.AddRange(horariosBloque);
                    }
                    var ids = horarios.Select(h => h.IdHorarioCancha).ToList();
                    if (ids.Count == 0 || ids.Distinct().Count() != ids.Count)
                    {
                        response.AddErrorResult("Los horarios no son válidos o se superponen.");
                        return response;
                    }
                    var cambioHorarios = !ids.OrderBy(i => i).SequenceEqual(reserva.DetalleReserva.Select(d => d.IdHorarioCancha ?? 0).OrderBy(i => i));
                    var hoy = DateTimeHelper.ObtenerAhoraLocal(TimezoneUtils.ObtenerZonaHoraria(reserva.IdCanchaNavigation.ZonaHoraria ?? "America/Lima")).Date;
                    if (cambioHorarios && reserva.FechaReserva.Date < hoy)
                    {
                        response.AddErrorResult("No se pueden cambiar horarios de días anteriores a hoy.");
                        return response;
                    }
                    var total = cambioHorarios ? horarios.Sum(h => h.PrecioHora) : pago.Monto;
                    if (total < pago.MontoAdelanto)
                    {
                        response.AddErrorResult("El nuevo total es menor que lo cobrado. Resuelve la devolución antes de reducir los horarios.");
                        return response;
                    }
                    pago.Monto = total;
                    pago.MontoPendiente = total - pago.MontoAdelanto;
                    reserva.MontoTotal = total;
                    var codigoPago = pago.MontoAdelanto == 0 ? Constants.ESTADO_PAGO.Pendiente :
                        pago.MontoAdelanto == total ? Constants.ESTADO_PAGO.Pagado : Constants.ESTADO_PAGO.Parcial;
                    var estadoPago = await _estadoPagoRepository.GetByAsync(e => e.Codigo == codigoPago && e.Activo);
                    if (estadoPago == null) throw new InvalidOperationException("Estado de pago no configurado.");
                    pago.IdEstadoPago = estadoPago.IdEstadoPago;
                    pago.IdEstadoPagoNavigation = estadoPago;
                    // Validar disponibilidad de horarios nuevos
                    var validacion = await ValidarDisponibilidadHorariosNuevos(
                        horarios, reserva.DetalleReserva.ToList(), dto.FechaReserva);

                    if (!validacion.Item1)
                    {
                        response.AddErrorResult(validacion.Item2);
                        return response;
                    }

                    ActualizarDetallesReserva(reserva, horarios);
                    ActualizarFechaReservaConPrimerHorario(reserva, dto.FechaReserva, horarios);
                }
                else if (reserva.DetalleReserva.Any())
                {
                    var idsActuales = reserva.DetalleReserva.Select(d => d.IdHorarioCancha).ToList();
                    horarios.AddRange(await _horarioCanchaRepository.FindByAsync(h => idsActuales.Contains(h.IdHorarioCancha),
                        h => h.IdHoraInicioNavigation, h => h.IdHoraFinNavigation!));
                    var horariosPorId = horarios.ToDictionary(h => h.IdHorarioCancha);
                    foreach (var detalle in reserva.DetalleReserva)
                        if (horariosPorId.TryGetValue(detalle.IdHorarioCancha ?? 0, out var horario))
                            detalle.IdHorarioCanchaNavigation = horario;
                }

                if (pago.IdEstadoPagoNavigation == null)
                    pago.IdEstadoPagoNavigation = (await _estadoPagoRepository.GetByAsync(e => e.IdEstadoPago == pago.IdEstadoPago))!;

                pago.Revision++;
                await _reservaRepository.UpdateAsync(reserva);
                await _reservaRepository.SaveAsync();

                var configuracion = reserva.IdCanchaNavigation.IdProveedorNavigation?.ConfiguracionProveedor;
                var reservaDto = ReservaDetalleDtoMapper.Map(_mapper!, reserva, reserva.DetalleReserva, pago,
                    configuracion?.Activo == true ? configuracion : null);
                response.UpdateData(reservaDto);

                response.AddOkResult(Resources.Common.UpdateSuccessMessage);
            }
            catch (Exception ex)
            {
                response.AddErrorResult($"Error al actualizar reserva: {ex.Message}");
            }

            return await Task.FromResult(response);
        }

        private static void ActualizarDetallesReserva(Entity.Reserva reserva, List<Entity.HorarioCancha> horarios)
        {
            var horariosPorId = horarios.ToDictionary(h => h.IdHorarioCancha);
            var detallesExistentes = reserva.DetalleReserva.ToList();
            foreach (var detalle in detallesExistentes)
            {
                detalle.Activo = horariosPorId.TryGetValue(detalle.IdHorarioCancha ?? 0, out var horario);
                if (horario != null) detalle.IdHorarioCanchaNavigation = horario;
            }
            var idsExistentes = detallesExistentes.Select(d => d.IdHorarioCancha).ToHashSet();
            foreach (var horario in horarios.Where(h => !idsExistentes.Contains(h.IdHorarioCancha)))
            {
                reserva.DetalleReserva.Add(new Entity.DetalleReserva
                {
                    IdReserva = reserva.IdReserva, IdHorarioCancha = horario.IdHorarioCancha,
                    IdHorarioCanchaNavigation = horario, Activo = true
                });
            }
        }

        private async Task<(bool, string)> ValidarDisponibilidadHorariosNuevos(
            List<Entity.HorarioCancha> horarios,
            List<Entity.DetalleReserva> detallesActuales, DateTimeOffset fechaReserva)
        {
            var idsActuales = detallesActuales.Select(d => d.IdHorarioCancha).ToHashSet();
            foreach (var horarioCancha in horarios.Where(h => !idsActuales.Contains(h.IdHorarioCancha)))
            {
                var todasReservas = await _detalleReservaRepository.FindByAsync(dr =>
                    dr.IdHorarioCancha == horarioCancha.IdHorarioCancha
                    && dr.Activo
                    && dr.IdReservaNavigation != null
                    && dr.IdReservaNavigation.Activo
                    && (dr.IdReservaNavigation.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Confirmado
                        || dr.IdReservaNavigation.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Pendiente),
                    dr => dr.IdReservaNavigation!,
                    dr => dr.IdReservaNavigation!.IdEstadoReservaNavigation);

                var reservasExistentes = todasReservas
                    .Where(dr => DateTimeHelper.NormalizarFechaUtc(dr.IdReservaNavigation!.FechaReserva).Date == fechaReserva.Date)
                    .ToList();

                if (reservasExistentes.Any())
                {
                    var hora = horarioCancha.IdHoraInicioNavigation?.HoraTexto ?? horarioCancha.IdHoraInicio.ToString();
                    return (false, $"El horario del {fechaReserva:dd/MM/yyyy} a las {hora} ya está reservado");
                }
            }

            return (true, string.Empty);
        }

        private static void ActualizarFechaReservaConPrimerHorario(Entity.Reserva reserva, DateTimeOffset fechaOriginal,
            List<Entity.HorarioCancha> horarios)
        {
            var primeraHora = horarios.Select(h => h.IdHoraInicioNavigation).Where(h => h != null)
                .OrderBy(h => h.Hora1).FirstOrDefault();
            if (primeraHora != null)
            {
                var horaInicio = primeraHora.Hora1;
                var fechaNormalizada = DateTimeHelper.NormalizarFechaUtc(fechaOriginal);

                reserva.FechaReserva = new DateTimeOffset(
                    fechaNormalizada.Year,
                    fechaNormalizada.Month,
                    fechaNormalizada.Day,
                    horaInicio.Hour,
                    horaInicio.Minute,
                    horaInicio.Second,
                    TimeSpan.Zero); // UTC offset
            }
        }
    }
}
