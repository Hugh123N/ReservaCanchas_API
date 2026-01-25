using AutoMapper;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Reserva;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using Reserva.Repository.Security;

namespace Reserva.Domain.Commands.Dbo.Reserva
{
    public class UpdateReservaCommandHandler : CommandHandlerBase<UpdateReservaCommand, GetReservaDto>
    {
        private readonly IRepository<Entity.Reserva> _reservaRepository;
        private readonly IRepository<Entity.DetalleReserva> _detalleReservaRepository;
        private readonly IRepository<Entity.HorarioCancha> _horarioCanchaRepository;
        private readonly IRepository<Entity.EstadoReserva> _estadoReservaRepository;
        private readonly IRepository<Entity.EstadoPago> _estadoPagoRepository;
        private readonly IRepository<Entity.Operador> _operadorRepository;
        private readonly IUserIdentity _userIdentity;

        public UpdateReservaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateReservaCommandValidator validator,
            IRepository<Entity.Reserva> reservaRepository,
            IRepository<Entity.DetalleReserva> detalleReservaRepository,
            IRepository<Entity.HorarioCancha> horarioCanchaRepository,
            IRepository<Entity.EstadoReserva> estadoReservaRepository,
            IRepository<Entity.EstadoPago> estadoPagoRepository,
            IRepository<Entity.Operador> operadorRepository,
            IUserIdentity userIdentity
        ) : base(unitOfWork, mapper, validator)
        {
            _reservaRepository = reservaRepository;
            _detalleReservaRepository = detalleReservaRepository;
            _horarioCanchaRepository = horarioCanchaRepository;
            _estadoReservaRepository = estadoReservaRepository;
            _estadoPagoRepository = estadoPagoRepository;
            _operadorRepository = operadorRepository;
            _userIdentity = userIdentity;
        }

        public override async Task<ResponseDto<GetReservaDto>> HandleCommand(UpdateReservaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetReservaDto>();
            var dto = request.UpdateDto;

            try
            {
                var reserva = await _reservaRepository.GetByAsync(x => x.IdReserva == dto.IdReserva && x.Activo,
                    x => x.DetalleReserva.Where(x => x.Activo),
                    x => x.Pago.Where(x => x.Activo)
                );

                if (reserva == null)
                {
                    response.AddErrorResult("Reserva no encontrada");
                    return response;
                }

                _mapper?.Map(dto, reserva);

                if (!string.IsNullOrEmpty(dto.CodigoEstadoReserva))
                {
                    var estadoReserva = await _estadoReservaRepository.GetByAsNoTrackingAsync(x => x.Codigo == dto.CodigoEstadoReserva && x.Activo);

                    if (estadoReserva == null)
                    {
                        response.AddErrorResult($"Estado de reserva '{dto.CodigoEstadoReserva}' no encontrado");
                        return response;
                    }

                    reserva.IdEstadoReserva = estadoReserva.IdEstadoReserva;

                    // Si es CONFIRMADO, actualizar operador y fecha
                    if (dto.CodigoEstadoReserva == Constants.ESTADO_RESERVA.Confirmado)
                    {
                        var idUserCurrent = _userIdentity.GetCurrentUserId();
                        if (idUserCurrent != null && idUserCurrent != Guid.Empty)
                        {
                            var operador = await _operadorRepository.GetByAsync(x => x.IdUsuario == idUserCurrent && x.Activo);

                            reserva.IdOperadorConfirmo = operador?.IdOperador;
                            reserva.FechaConfirmacion = DateTimeOffset.UtcNow;
                        }
                    }
                }

                if (dto.PagoReserva != null && dto.PagoReserva.Any())
                {
                    var pagoDto = dto.PagoReserva.First();
                    var pagoExistente = reserva.Pago.FirstOrDefault();

                    if (pagoExistente != null)
                    {
                        await ActualizarPago(pagoExistente, pagoDto, dto.MontoTotal);
                    }
                }

                if (dto.Horarios != null && dto.Horarios.Any())
                {
                    // Validar disponibilidad de horarios nuevos
                    var validacion = await ValidarDisponibilidadHorariosNuevos(
                        dto.IdCancha, dto.Horarios, reserva.DetalleReserva.ToList());

                    if (!validacion.Item1)
                    {
                        response.AddErrorResult(validacion.Item2);
                        return response;
                    }

                    await ActualizarDetallesReserva(reserva, dto.IdCancha, dto.Horarios);
                }

                

                // 6. Guardar cambios
                await _reservaRepository.UpdateAsync(reserva);
                await _reservaRepository.SaveAsync();

                // 7. Retornar respuesta
                var reservaDto = _mapper?.Map<GetReservaDto>(reserva);
                if (reservaDto != null) response.UpdateData(reservaDto);

                response.AddOkResult(Resources.Common.UpdateSuccessMessage);
            }
            catch (Exception ex)
            {
                response.AddErrorResult($"Error al actualizar reserva: {ex.Message}");
            }

            return await Task.FromResult(response);
        }

        private async Task ActualizarPago(Entity.Pago pago, Dto.Dbo.Pago.UpdatePagoDto pagoDto, decimal montoTotal)
        {
            decimal montoPagado = pagoDto.MontoAdelanto;
            decimal montoPendiente = montoTotal - montoPagado;

            string codigoEstadoPago;
            if (montoPagado == 0)
            {
                codigoEstadoPago = Constants.ESTADO_PAGO.Pendiente;
            }
            else if (montoPagado >= montoTotal)
            {
                codigoEstadoPago = Constants.ESTADO_PAGO.Pagado;
            }
            else
            {
                codigoEstadoPago = Constants.ESTADO_PAGO.Parcial;
            }

            var estadoPago = await _estadoPagoRepository.GetByAsync(x => x.Codigo == codigoEstadoPago && x.Activo);

            pago.Monto = montoTotal;
            pago.MontoAdelanto = montoPagado;
            pago.MontoPendiente = montoPendiente;
            pago.IdEstadoPago = estadoPago?.IdEstadoPago ?? pago.IdEstadoPago;
            pago.CodigoOperacion = pagoDto.CodigoOperacion;
            pago.NumeroReferencia = pagoDto.NumeroReferencia;
        }

        private async Task ActualizarDetallesReserva(Entity.Reserva reserva, int idCancha, List<Dto.Dbo.Calendario.BloqueHorarioDto> bloquesNuevos)
        {
            // Expandir bloques a IDs de HorarioCancha
            var idsHorariosNuevos = new List<int>();
            foreach (var bloque in bloquesNuevos)
            {
                var diaSemana = (int)bloque.Fecha.DayOfWeek;
                if (diaSemana == 0) diaSemana = 7;

                var horariosEnRango = await _horarioCanchaRepository.FindByAsync(
                    hc => hc.IdHorarioCancha >= bloque.IdHorarioCanchaInicio
                       && hc.IdHorarioCancha <= bloque.IdHorarioCanchaFin
                       && hc.IdCancha == idCancha
                       && hc.IdDiaSemana == diaSemana
                       && hc.Activo);

                idsHorariosNuevos.AddRange(horariosEnRango.Select(h => h.IdHorarioCancha));
            }

            var detallesExistentes = reserva.DetalleReserva.ToList();

            // Marcar como inactivos los que ya no están
            foreach (var detalle in detallesExistentes)
            {
                if (!idsHorariosNuevos.Contains(detalle.IdHorarioCancha ?? 0))
                {
                    detalle.Activo = false;
                }
            }

            // Agregar los nuevos
            var idsExistentes = detallesExistentes.Select(d => d.IdHorarioCancha).ToList();
            foreach (var idHorarioNuevo in idsHorariosNuevos)
            {
                if (!idsExistentes.Contains(idHorarioNuevo))
                {
                    var nuevoDetalle = new Entity.DetalleReserva
                    {
                        IdReserva = reserva.IdReserva,
                        IdHorarioCancha = idHorarioNuevo,
                        Activo = true
                    };

                    reserva.DetalleReserva.Add(nuevoDetalle);
                }
            }
        }

        private async Task<(bool, string)> ValidarDisponibilidadHorariosNuevos(
            int idCancha,
            List<Dto.Dbo.Calendario.BloqueHorarioDto> bloquesNuevos,
            List<Entity.DetalleReserva> detallesActuales)
        {
            var idsActuales = detallesActuales.Select(d => d.IdHorarioCancha).ToList();

            foreach (var bloque in bloquesNuevos)
            {
                var diaSemana = (int)bloque.Fecha.DayOfWeek;
                if (diaSemana == 0) diaSemana = 7;

                var horariosEnRango = await _horarioCanchaRepository.FindByAsync(
                    hc => hc.IdHorarioCancha >= bloque.IdHorarioCanchaInicio
                       && hc.IdHorarioCancha <= bloque.IdHorarioCanchaFin
                       && hc.IdCancha == idCancha
                       && hc.IdDiaSemana == diaSemana
                       && hc.Activo,
                    hc => hc.IdHoraInicioNavigation);

                if (!horariosEnRango.Any())
                {
                    return (false, $"No se encontraron horarios configurados para el rango seleccionado");
                }

                foreach (var horarioCancha in horariosEnRango.OrderBy(h => h.IdHoraInicio))
                {
                    // Si este horario ya existía en la reserva, no validar
                    if (idsActuales.Contains(horarioCancha.IdHorarioCancha))
                        continue;

                    // Validar que no esté reservado por otra reserva
                    var reservasExistentes = await _detalleReservaRepository.FindByAsync(dr =>
                        dr.IdHorarioCancha == horarioCancha.IdHorarioCancha
                        && dr.Activo
                        && dr.IdReservaNavigation != null
                        && dr.IdReservaNavigation.Activo
                        && dr.IdReservaNavigation.FechaReserva.Date == bloque.Fecha.Date
                        && (dr.IdReservaNavigation.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Confirmado
                            || dr.IdReservaNavigation.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Pendiente),
                        dr => dr.IdReservaNavigation!,
                        dr => dr.IdReservaNavigation!.IdEstadoReservaNavigation);

                    if (reservasExistentes.Any())
                    {
                        var hora = horarioCancha.IdHoraInicioNavigation?.HoraTexto ?? horarioCancha.IdHoraInicio.ToString();
                        return (false, $"El horario del {bloque.Fecha:dd/MM/yyyy} a las {hora} ya está reservado");
                    }
                }
            }

            return (true, string.Empty);
        }
    }
}
