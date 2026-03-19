using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Calendario;
using Reserva.Dto.Dbo.Hora;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Security;
using Reserva.Repository.Utils;
using System.Globalization;

namespace Reserva.Domain.Queries.Dbo.Calendario
{
    public class GetDisponibilidadSemanalQueryHandler : QueryHandlerBase<GetDisponibilidadSemanalQuery, DisponibilidadSemanalResponseDto>
    {
        private readonly IRepository<Entity.Cancha> _canchaRepository;
        private readonly IRepository<Entity.HorarioCancha> _horarioCanchaRepository;
        private readonly IRepository<Entity.Reserva> _reservaRepository;
        private readonly IRepository<Entity.DetalleReserva> _detalleReservaRepository;

        public GetDisponibilidadSemanalQueryHandler(
            IMapper mapper,
            IMediator mediator,
            GetDisponibilidadSemanalQueryValidator validator,
            IRepository<Entity.Cancha> canchaRepository,
            IRepository<Entity.HorarioCancha> horarioCanchaRepository,
            IRepository<Entity.Reserva> reservaRepository,
            IRepository<Entity.DetalleReserva> detalleReservaRepository
        ) : base(mapper, mediator, validator)
        {
            _canchaRepository = canchaRepository;
            _horarioCanchaRepository = horarioCanchaRepository;
            _reservaRepository = reservaRepository;
            _detalleReservaRepository = detalleReservaRepository;
        }

        protected override async Task<ResponseDto<DisponibilidadSemanalResponseDto>> HandleQuery(GetDisponibilidadSemanalQuery request,CancellationToken cancellationToken)
        {
            var response = new ResponseDto<DisponibilidadSemanalResponseDto>();

            try
            {
                var cancha = await _canchaRepository.GetByAsync(c => c.IdCancha == request.IdCancha);

                var zonaHoraria = TimezoneUtils.ObtenerZonaHoraria(cancha!.ZonaHoraria);

                var horariosCancha = await _horarioCanchaRepository.FindByAsNoTrackingAsync(hc => hc.IdCancha == request.IdCancha && hc.Activo,
                    hc => hc.IdHoraInicioNavigation,
                    hc => hc.IdHoraFinNavigation!,
                    hc => hc.IdDiaSemanaNavigation);

                if (!horariosCancha.Any())
                {
                    response.AddErrorResult("La cancha no tiene horarios configurados");
                    return response;
                }

                //Determinar hora mínima y máxima
                var horaMinima = horariosCancha.Min(hc => hc.IdHoraInicioNavigation.HoraTexto);
                var ultimoHorario = horariosCancha
                    .OrderByDescending(hc => hc.IdHoraInicioNavigation.HoraTexto).First();
                var horaMaxima = ultimoHorario.IdHoraFinNavigation?.HoraTexto
                    ?? TimeOnly.Parse(ultimoHorario.IdHoraInicioNavigation.HoraTexto).AddMinutes(30).ToString();

                var fechaInicioLocal = DateTimeHelper.NormalizarFechaLocal(request.FechaInicio, zonaHoraria);
                var fechaFinLocal = DateTimeHelper.NormalizarFechaLocal(request.FechaFin, zonaHoraria);

                var fechaInicioUtc = fechaInicioLocal.ToUniversalTime();
                var fechaFinUtc = fechaFinLocal.AddDays(1).AddSeconds(-1).ToUniversalTime();

                var reservas = await _reservaRepository.FindByAsNoTrackingAsync(r => r.Activo && r.IdCancha == request.IdCancha &&
                    r.FechaReserva >= fechaInicioUtc &&
                    r.FechaReserva <= fechaFinUtc &&
                    (r.IdEstadoReservaNavigation.Codigo.Equals(Constants.ESTADO_RESERVA.Pendiente) || r.IdEstadoReservaNavigation.Codigo.Equals(Constants.ESTADO_RESERVA.Confirmado)),
                    r => r.IdClienteNavigation,
                    r => r.IdEstadoReservaNavigation);

                //Crear diccionario de reservas por IdHorarioCancha y fecha
                var reservaIds = reservas.Select(r => r.IdReserva).ToList();
                var detallesReserva = await _detalleReservaRepository.FindByAsNoTrackingAsync(dr => reservaIds.Contains(dr.IdReserva) && dr.Activo && dr.IdHorarioCancha.HasValue);

                var detallesPorReserva = detallesReserva.GroupBy(d => d.IdReserva).ToDictionary(g => g.Key, g => g.ToList());

                var reservasPorHorarioYFecha = new Dictionary<string, Entity.Reserva>();

                foreach (var reserva in reservas)
                {
                    if (!detallesPorReserva.TryGetValue(reserva.IdReserva, out var detalles))
                        continue;

                    var fechaReservaNormalizada = DateTimeHelper.NormalizarFechaLocal(reserva.FechaReserva, zonaHoraria);

                    foreach (var detalle in detalles)
                    {
                        var key = $"{detalle.IdHorarioCancha!.Value}_{fechaReservaNormalizada:yyyy-MM-dd}";

                        if (!reservasPorHorarioYFecha.ContainsKey(key))
                        {
                            reservasPorHorarioYFecha[key] = reserva;
                        }
                    }
                }

                //Generar disponibilidad para cada día de la semana
                var diasHorarios = new List<DiaHorarioDto>();
                var fechaActual = request.FechaInicio.Date;

                while (fechaActual <= request.FechaFin.Date)
                {
                    var diaSemana = (int)fechaActual.DayOfWeek;
                    var idDiaSemana = diaSemana == 0 ? 7 : diaSemana;

                    var horariosDelDia = horariosCancha
                        .Where(hc => hc.IdDiaSemana == idDiaSemana)
                        .OrderBy(hc => hc.IdHoraInicioNavigation.HoraTexto)
                        .ToList();

                    var slots = new List<SlotHorarioDto>();

                    foreach (var horario in horariosDelDia)
                    {
                        var horaInicio = TimeOnly.Parse(horario.IdHoraInicioNavigation.HoraTexto);
                        var horaFin = horario.IdHoraFinNavigation != null
                            ? TimeOnly.Parse(horario.IdHoraFinNavigation.HoraTexto) : horaInicio.AddMinutes(30);

                        var fechaBase = fechaActual.Date;

                        var inicio = fechaBase.Add(horaInicio.ToTimeSpan());
                        var fin = fechaBase.Add(horaFin.ToTimeSpan());

                        // Si cruza medianoche (23:00 → 00:00, 01:00, etc.)
                        if (fin <= inicio)
                        {
                            fin = fin.AddDays(1);
                        }

                        // Generar 1 solo slot por cada HorarioCancha (sin subdividir)
                        var key = $"{horario.IdHorarioCancha}_{fechaBase:yyyy-MM-dd}";
                        var tieneReserva = reservasPorHorarioYFecha.TryGetValue(key, out var reserva);

                        slots.Add(new SlotHorarioDto
                        {
                            IdHorarioCancha = horario.IdHorarioCancha,
                            Hora = inicio.ToString("HH:mm"),
                            HoraFin = fin.ToString("HH:mm"),
                            Estado = tieneReserva
                                ? (reserva!.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Pendiente
                                    ? "PENDIENTE" : "CONFIRMADO")
                                : "DISPONIBLE",
                            Precio = horario.PrecioHora,
                            Reserva = tieneReserva ? await MapearReservaSlot(reserva!) : null
                        });
                    }

                    var diaHorario = new DiaHorarioDto
                    {
                        Fecha = fechaActual.ToString("yyyy-MM-dd"),
                        DiaSemana = idDiaSemana,
                        NombreDia = CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(fechaActual.DayOfWeek),
                        Slots = slots
                    };

                    diasHorarios.Add(diaHorario);
                    fechaActual = fechaActual.AddDays(1);
                }

                //Construir respuesta
                var resultado = new DisponibilidadSemanalResponseDto
                {
                    Cancha = new CanchaDisponibilidadDto
                    {
                        IdCancha = cancha!.IdCancha,
                        Nombre = cancha.Nombre,
                        HoraInicio = horaMinima!,
                        HoraFin = horaMaxima
                    },
                    Horarios = diasHorarios
                };

                response.UpdateData(resultado);
            }
            catch (Exception ex)
            {
                response.AddErrorResult("Error al obtener la disponibilidad semanal: ", ex);
            }

            return await Task.FromResult(response);
        }

        private async Task<ReservaSlotDto> MapearReservaSlot(Entity.Reserva reserva)
        {
            return new ReservaSlotDto
            {
                IdReserva = reserva.IdReserva,
                CodigoReserva = reserva.CodigoReserva ?? string.Empty,
                Cliente = new ClienteSlotDto
                {
                    IdCliente = reserva.IdCliente.ToString(),
                    Nombre = $"{reserva.IdClienteNavigation.FirstName} {reserva.IdClienteNavigation.LastName}".Trim(),
                    Telefono = reserva.IdClienteNavigation.PhoneNumber ?? "",
                    Email = reserva.IdClienteNavigation.Email ?? ""
                },
            };
        }
    }
}
