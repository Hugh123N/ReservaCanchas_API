using AutoMapper;
using MediatR;
using Reserva.Common;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Calendario;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Utils;

namespace Reserva.Domain.Queries.Dbo.Calendario
{
    public class ValidarDisponibilidadQueryHandler : QueryHandlerBase<ValidarDisponibilidadQuery, ValidarDisponibilidadResponseDto>
    {
        private readonly IRepository<Entity.HorarioCancha> _horarioCanchaRepository;
        private readonly IRepository<Entity.DetalleReserva> _detalleReservaRepository;
        private readonly IRepository<Entity.Hora> _horaRepository;
        private readonly IRepository<Entity.Cancha> _canchaRepository;

        public ValidarDisponibilidadQueryHandler(
            IMapper mapper,
            IMediator mediator,
            IRepository<Entity.HorarioCancha> horarioCanchaRepository,
            IRepository<Entity.DetalleReserva> detalleReservaRepository,
            IRepository<Entity.Hora> horaRepository,
            IRepository<Entity.Cancha> canchaRepository) : base(mapper, mediator)
        {
            _horarioCanchaRepository = horarioCanchaRepository;
            _detalleReservaRepository = detalleReservaRepository;
            _horaRepository = horaRepository;
            _canchaRepository = canchaRepository;
        }

        protected override async Task<ResponseDto<ValidarDisponibilidadResponseDto>> HandleQuery(ValidarDisponibilidadQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<ValidarDisponibilidadResponseDto>();
            var horariosNoDisponibles = new List<HorarioNoDisponibleDto>();

            try
            {
                var cancha = await _canchaRepository.GetByAsync(c => c.IdCancha == request.IdCancha && c.Activo);
                var zonaHoraria = TimezoneUtils.ObtenerZonaHoraria(cancha!.ZonaHoraria);

                var fechaReserva = await FechaReservaConPrimerHorario(request.Horarios, zonaHoraria);
                foreach (var bloque in request.Horarios)
                {
                    var fechaNormalizada = DateTimeHelper.NormalizarFechaLocal(bloque.Fecha, zonaHoraria);

                    var diaSemana = (int)fechaNormalizada.DayOfWeek;
                    if (diaSemana == 0) diaSemana = 7;

                    var horariosEnRango = await _horarioCanchaRepository.FindByAsync(
                        hc => hc.IdHorarioCancha >= bloque.IdHorarioCanchaInicio
                           && hc.IdHorarioCancha <= bloque.IdHorarioCanchaFin
                           && hc.IdCancha == request.IdCancha
                           && hc.IdDiaSemana == diaSemana
                           && hc.Activo,
                        hc => hc.IdHoraInicioNavigation,
                        hc => hc.IdHoraFinNavigation!);

                    if (!horariosEnRango.Any())
                    {
                        horariosNoDisponibles.Add(new HorarioNoDisponibleDto
                        {
                            Fecha = fechaNormalizada,
                            IdHoraInicio = 0,
                            IdHoraFin = 0,
                            HoraInicio = "N/A",
                            HoraFin = "N/A",
                            Motivo = "No se encontraron horarios configurados para el rango seleccionado"
                        });
                        continue;
                    }

                    foreach (var horarioCancha in horariosEnRango.OrderBy(h => h.IdHoraInicio))
                    {
                        // Obtener todas las reservas para este horario
                        var todasReservas = await _detalleReservaRepository.FindByAsync(dr =>
                            dr.IdHorarioCancha == horarioCancha.IdHorarioCancha
                            && dr.Activo
                            && dr.IdReservaNavigation != null
                            && dr.IdReservaNavigation.Activo
                            && (dr.IdReservaNavigation.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Pendiente
                             || dr.IdReservaNavigation.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Confirmado),
                            dr => dr.IdReservaNavigation!,
                            dr => dr.IdReservaNavigation!.IdEstadoReservaNavigation);

                        var reservasExistentes = todasReservas
                            .Where(dr => DateTimeHelper.NormalizarFechaLocal(dr.IdReservaNavigation!.FechaReserva, zonaHoraria).Date == fechaReserva.Date)
                            .ToList();

                        if (reservasExistentes.Any())
                        {
                            var horaInicio = horarioCancha.IdHoraInicioNavigation?.HoraTexto ?? horarioCancha.IdHoraInicio.ToString();
                            var horaFin = horarioCancha.IdHoraFinNavigation?.HoraTexto ?? horarioCancha.IdHoraFin.ToString();

                            horariosNoDisponibles.Add(new HorarioNoDisponibleDto
                            {
                                Fecha = fechaNormalizada,
                                IdHoraInicio = horarioCancha.IdHoraInicio,
                                IdHoraFin = horarioCancha.IdHoraFin ?? 0,
                                HoraInicio = horaInicio,
                                HoraFin = horaFin!,
                                Motivo = "Ya está reservado"
                            });
                        }
                    }
                }

                var resultado = new ValidarDisponibilidadResponseDto
                {
                    TodosDisponibles = !horariosNoDisponibles.Any(),
                    HorariosNoDisponibles = horariosNoDisponibles,
                    Mensaje = horariosNoDisponibles.Any()
                        ? $"Se encontraron {horariosNoDisponibles.Count} horario(s) no disponible(s)"
                        : "Todos los horarios están disponibles"
                };

                response.UpdateData(resultado);
                response.AddOkResult(resultado.Mensaje);
            }
            catch (Exception ex)
            {
                response.AddErrorResult($"Error al validar disponibilidad: {ex.Message}");
            }

            return response;
        }

        private async Task<DateTimeOffset> FechaReservaConPrimerHorario(List<BloqueHorarioDto> horarios, TimeZoneInfo zonaHoraria)
        {
            var primeraFechaDto = horarios.Min(h => h.Fecha);
            var primeraFecha = DateTimeHelper.NormalizarFechaLocal(primeraFechaDto, zonaHoraria);

            var primerBloque = horarios
                    .OrderBy(h => h.Fecha)
                    .ThenBy(h => h.IdHorarioCanchaInicio)
                    .First();

            var primerHorarioCancha = await _horarioCanchaRepository.GetByAsync(
                hc => hc.IdHorarioCancha == primerBloque.IdHorarioCanchaInicio && hc.Activo,
                hc => hc.IdHoraInicioNavigation);

            if (primerHorarioCancha != null && primerHorarioCancha.IdHoraInicioNavigation != null)
            {
                var horaInicio = primerHorarioCancha.IdHoraInicioNavigation.Hora1;
                var offsetLocal = zonaHoraria.GetUtcOffset(primeraFecha.DateTime);

                return new DateTimeOffset(
                    primeraFecha.Year,
                    primeraFecha.Month,
                    primeraFecha.Day,
                    horaInicio.Hour,
                    horaInicio.Minute,
                    horaInicio.Second,
                    offsetLocal);
            }

            return primeraFecha;
        }
    }
}
