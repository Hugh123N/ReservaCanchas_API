using AutoMapper;
using MediatR;
using Reserva.Common;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Calendario;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Calendario
{
    public class ValidarDisponibilidadQueryHandler : QueryHandlerBase<ValidarDisponibilidadQuery, ValidarDisponibilidadResponseDto>
    {
        private readonly IRepository<Entity.HorarioCancha> _horarioCanchaRepository;
        private readonly IRepository<Entity.DetalleReserva> _detalleReservaRepository;
        private readonly IRepository<Entity.Hora> _horaRepository;

        public ValidarDisponibilidadQueryHandler(
            IMapper mapper,
            IMediator mediator,
            IRepository<Entity.HorarioCancha> horarioCanchaRepository,
            IRepository<Entity.DetalleReserva> detalleReservaRepository,
            IRepository<Entity.Hora> horaRepository) : base(mapper, mediator)
        {
            _horarioCanchaRepository = horarioCanchaRepository;
            _detalleReservaRepository = detalleReservaRepository;
            _horaRepository = horaRepository;
        }

        protected override async Task<ResponseDto<ValidarDisponibilidadResponseDto>> HandleQuery(
            ValidarDisponibilidadQuery request,
            CancellationToken cancellationToken)
        {
            var response = new ResponseDto<ValidarDisponibilidadResponseDto>();
            var horariosNoDisponibles = new List<HorarioNoDisponibleDto>();

            try
            {
                foreach (var bloque in request.Horarios)
                {
                    var diaSemana = (int)bloque.Fecha.DayOfWeek;
                    if (diaSemana == 0) diaSemana = 7;

                    var horariosEnRango = await _horarioCanchaRepository.FindByAsync(
                        hc => hc.IdHorarioCancha >= bloque.IdHorarioCanchaInicio
                           && hc.IdHorarioCancha <= bloque.IdHorarioCanchaFin
                           && hc.IdCancha == request.IdCancha
                           && hc.IdDiaSemana == diaSemana
                           && hc.Activo,
                        hc => hc.IdHoraInicioNavigation,
                        hc => hc.IdHoraFinNavigation);

                    if (!horariosEnRango.Any())
                    {
                        horariosNoDisponibles.Add(new HorarioNoDisponibleDto
                        {
                            Fecha = bloque.Fecha,
                            IdHoraInicio = 0,
                            IdHoraFin = 0,
                            HoraInicio = "N/A",
                            HoraFin = "N/A",
                            Motivo = "No se encontraron horarios configurados para el rango seleccionado"
                        });
                        continue;
                    }

                    // Verificar disponibilidad de cada horario
                    foreach (var horarioCancha in horariosEnRango.OrderBy(h => h.IdHoraInicio))
                    {
                        // Verificar si ya está reservado
                        var reservasExistentes = await _detalleReservaRepository.FindByAsync(dr =>
                            dr.IdHorarioCancha == horarioCancha.IdHorarioCancha
                            && dr.Activo
                            && dr.IdReservaNavigation != null
                            && dr.IdReservaNavigation.Activo
                            && dr.IdReservaNavigation.FechaReserva.Date == bloque.Fecha.Date
                            && (dr.IdReservaNavigation.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Pendiente
                             || dr.IdReservaNavigation.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Confirmado),
                            dr => dr.IdReservaNavigation!,
                            dr => dr.IdReservaNavigation!.IdEstadoReservaNavigation);

                        if (reservasExistentes.Any())
                        {
                            var horaInicio = horarioCancha.IdHoraInicioNavigation?.HoraTexto ?? horarioCancha.IdHoraInicio.ToString();
                            var horaFin = horarioCancha.IdHoraFinNavigation?.HoraTexto ?? horarioCancha.IdHoraFin.ToString();

                            horariosNoDisponibles.Add(new HorarioNoDisponibleDto
                            {
                                Fecha = bloque.Fecha,
                                IdHoraInicio = horarioCancha.IdHoraInicio,
                                IdHoraFin = horarioCancha.IdHoraFin ?? 0,
                                HoraInicio = horaInicio,
                                HoraFin = horaFin,
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
    }
}
