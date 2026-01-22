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
using System.Globalization;

namespace Reserva.Domain.Queries.Dbo.Calendario
{
    public class GetDisponibilidadSemanalQueryHandler : QueryHandlerBase<GetDisponibilidadSemanalQuery, DisponibilidadSemanalResponseDto>
    {
        private readonly IRepository<Entity.Cancha> _canchaRepository;
        private readonly IRepository<Entity.HorarioCancha> _horarioCanchaRepository;
        private readonly IRepository<Entity.Reserva> _reservaRepository;
        private readonly IRepository<Entity.DetalleReserva> _detalleReservaRepository;
        private readonly IRepository<Entity.AspNetUsers> _userRepository;
        private readonly IUserIdentity _userIdentity;

        public GetDisponibilidadSemanalQueryHandler(
            IMapper mapper,
            IMediator mediator,
            GetDisponibilidadSemanalQueryValidator validator,
            IRepository<Entity.Cancha> canchaRepository,
            IRepository<Entity.HorarioCancha> horarioCanchaRepository,
            IRepository<Entity.Reserva> reservaRepository,
            IRepository<Entity.DetalleReserva> detalleReservaRepository,
            IRepository<Entity.AspNetUsers> userRepository,
            IUserIdentity userIdentity
        ) : base(mapper, mediator, validator)
        {
            _canchaRepository = canchaRepository;
            _horarioCanchaRepository = horarioCanchaRepository;
            _reservaRepository = reservaRepository;
            _detalleReservaRepository = detalleReservaRepository;
            _userRepository = userRepository;
            _userIdentity = userIdentity;
        }

        protected override async Task<ResponseDto<DisponibilidadSemanalResponseDto>> HandleQuery(GetDisponibilidadSemanalQuery request,CancellationToken cancellationToken)
        {
            var response = new ResponseDto<DisponibilidadSemanalResponseDto>();

            try
            {
                var cancha = await _canchaRepository.GetByAsync(c => c.IdCancha == request.IdCancha);

                var horariosCancha = await _horarioCanchaRepository.FindByAsNoTrackingAsync(hc => hc.IdCancha == request.IdCancha && hc.Activo,
                    hc => hc.IdHoraInicioNavigation,
                    hc => hc.IdHoraFinNavigation,
                    hc => hc.IdDiaSemanaNavigation);

                if (!horariosCancha.Any())
                {
                    response.AddErrorResult("La cancha no tiene horarios configurados");
                    return response;
                }

                //Determinar hora mínima y máxima
                var horaMinima = horariosCancha
                    .Min(hc => hc.IdHoraInicioNavigation.HoraTexto);
                var ultimoHorario = horariosCancha
                    .OrderByDescending(hc => hc.IdHoraInicioNavigation.HoraTexto)
                    .First();
                var horaMaxima = ultimoHorario.IdHoraFinNavigation?.HoraTexto
                    ?? TimeOnly.Parse(ultimoHorario.IdHoraInicioNavigation.HoraTexto).AddMinutes(30).ToString();

                //Obtener reservas del rango de fechas (solo estados PENDIENTE='01' y CONFIRMADO='02')
                var fechaInicioOffset = new DateTimeOffset(request.FechaInicio.Date);
                var fechaFinOffset = new DateTimeOffset(request.FechaFin.Date.AddDays(1).AddSeconds(-1));

                var reservas = await _reservaRepository.FindAll()
                    .Where(r => r.IdCancha == request.IdCancha &&
                               r.FechaReserva >= fechaInicioOffset &&
                               r.FechaReserva <= fechaFinOffset &&
                               (r.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Pendiente || r.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Confirmado) && // PENDIENTE o CONFIRMADO
                               r.Activo)
                    .Include(r => r.IdClienteNavigation)
                    .Include(r => r.IdTipoDeporteNavigation)
                    .Include(r => r.IdEstadoReservaNavigation)
                    .Include(r => r.DetalleReserva)
                    .ToListAsync(cancellationToken);

                //Crear diccionario de reservas por IdHorarioCancha y fecha
                var reservaIds = reservas.Select(r => r.IdReserva).ToList();
                var detallesReserva = await _detalleReservaRepository.FindByAsNoTrackingAsync(dr => reservaIds.Contains(dr.IdReserva) && dr.Activo && dr.IdHorarioCancha.HasValue);
                
                var detallesPorReserva = detallesReserva.GroupBy(d => d.IdReserva).ToDictionary(g => g.Key, g => g.ToList());

                var reservasPorHorarioYFecha = new Dictionary<string, Entity.Reserva>();

                foreach (var reserva in reservas)
                {
                    if (!detallesPorReserva.TryGetValue(reserva.IdReserva, out var detalles))
                        continue;

                    var fechaReserva = reserva.FechaReserva.Date;

                    foreach (var detalle in detalles)
                    {
                        var key = $"{detalle.IdHorarioCancha!.Value}_{fechaReserva:yyyy-MM-dd}";

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
                                    ? "PENDIENTE"
                                    : "CONFIRMADO")
                                : "DISPONIBLE",
                            Precio = horario.PrecioHora,
                            Reserva = tieneReserva
                                ? await MapearReservaSlot(reserva!, horario.IdHorarioCancha, cancellationToken)
                                : null
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
                        IdCancha = cancha.IdCancha,
                        Nombre = cancha.Nombre,
                        HoraInicio = horaMinima,
                        HoraFin = horaMaxima
                    },
                    Horarios = diasHorarios
                };

                response.UpdateData(resultado);
            }
            catch (Exception ex)
            {
                response.AddErrorResult("Error al obtener la disponibilidad semanal", ex);
            }

            return await Task.FromResult(response);
        }

        private async Task<ReservaSlotDto> MapearReservaSlot(Entity.Reserva reserva,int idHorarioCanchaActual,
            CancellationToken cancellationToken)
        {
            // Obtener todos los detalles de la reserva ordenados por hora
            var detalles = await _detalleReservaRepository
                .FindAll()
                .Where(dr => dr.IdReserva == reserva.IdReserva && dr.Activo)
                .Include(dr => dr.IdHorarioCanchaNavigation)
                    .ThenInclude(hc => hc!.IdHoraInicioNavigation)
                .Include(dr => dr.IdHorarioCanchaNavigation)
                    .ThenInclude(hc => hc!.IdHoraFinNavigation)
                .OrderBy(dr => dr.IdHorarioCanchaNavigation!.IdHoraInicio)
                .ToListAsync(cancellationToken);

            // Encontrar el bloque consecutivo al que pertenece el horario actual
            var bloqueConsecutivo = EncontrarBloqueConsecutivo(detalles, idHorarioCanchaActual);

            return new ReservaSlotDto
            {
                IdReserva = reserva.IdReserva,
                CodigoReserva = reserva.CodigoReserva,
                Cliente = new ClienteSlotDto
                {
                    IdCliente = reserva.IdCliente.ToString(),
                    Nombre = reserva.IdClienteNavigation.UserName ?? "",
                    Telefono = reserva.IdClienteNavigation.PhoneNumber ?? "",
                    Email = reserva.IdClienteNavigation.Email ?? ""
                },
                Deporte = reserva.IdTipoDeporteNavigation.Nombre,
                HoraInicio = bloqueConsecutivo.HoraInicio,
                HoraFin = bloqueConsecutivo.HoraFin,
                CantidadHoras = bloqueConsecutivo.CantidadBloques * 0.5m,  // Bloques de 30min
                Monto = reserva.MontoTotal,
                FechaExpiracion = reserva.FechaExpiracionPreReserva?.DateTime,
                IdEstadoReserva = reserva.IdEstadoReservaNavigation.Codigo
            };
        }

        private (string HoraInicio, string HoraFin, int CantidadBloques) EncontrarBloqueConsecutivo(List<Entity.DetalleReserva> detalles,int idHorarioCanchaActual)
        {
            if (!detalles.Any())
            {
                return ("00:00", "00:00", 0);
            }

            var indiceActual = detalles.FindIndex(d => d.IdHorarioCancha == idHorarioCanchaActual);

            if (indiceActual == -1)
            {
                // Si no se encuentra, retornar el primer bloque
                var primerDetalle = detalles.First();
                var horaInicio = primerDetalle.IdHorarioCanchaNavigation!.IdHoraInicioNavigation!.HoraTexto;
                var horaFin = primerDetalle.IdHorarioCanchaNavigation!.IdHoraFinNavigation?.HoraTexto
                    ?? TimeOnly.Parse(horaInicio).AddMinutes(30).ToString("HH:mm");
                return (horaInicio, horaFin, 1);
            }

            // Buscar hacia atrás para encontrar el inicio del bloque consecutivo
            int inicioBloque = indiceActual;
            while (inicioBloque > 0)
            {
                var actual = detalles[inicioBloque];
                var anterior = detalles[inicioBloque - 1];

                // Verificar si son consecutivos (el IdHoraFin del anterior == IdHoraInicio del actual)
                var idHoraFinAnterior = anterior.IdHorarioCanchaNavigation!.IdHoraFin;
                var idHoraInicioActual = actual.IdHorarioCanchaNavigation!.IdHoraInicio;

                if (idHoraFinAnterior == idHoraInicioActual)
                {
                    inicioBloque--;
                }
                else
                {
                    break;
                }
            }

            // Buscar hacia adelante para encontrar el fin del bloque consecutivo
            int finBloque = indiceActual;
            while (finBloque < detalles.Count - 1)
            {
                var actual = detalles[finBloque];
                var siguiente = detalles[finBloque + 1];

                // Verificar si son consecutivos
                var idHoraFinActual = actual.IdHorarioCanchaNavigation!.IdHoraFin;
                var idHoraInicioSiguiente = siguiente.IdHorarioCanchaNavigation!.IdHoraInicio;

                if (idHoraFinActual == idHoraInicioSiguiente)
                {
                    finBloque++;
                }
                else
                {
                    break;
                }
            }

            // Obtener las horas del bloque
            var detalleInicio = detalles[inicioBloque];
            var detalleFin = detalles[finBloque];

            var horaInicioBloque = detalleInicio.IdHorarioCanchaNavigation!.IdHoraInicioNavigation!.HoraTexto;
            var horaFinBloque = detalleFin.IdHorarioCanchaNavigation!.IdHoraFinNavigation?.HoraTexto
                ?? TimeOnly.Parse(detalleFin.IdHorarioCanchaNavigation!.IdHoraInicioNavigation!.HoraTexto)
                    .AddMinutes(30).ToString("HH:mm");

            var cantidadBloques = finBloque - inicioBloque + 1;

            return (horaInicioBloque, horaFinBloque, cantidadBloques);
        }
    }
}
