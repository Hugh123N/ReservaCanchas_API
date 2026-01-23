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
        private readonly IRepository<Entity.Pago> _pagoRepository;
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
            IRepository<Entity.Pago> pagoRepository,
            IRepository<Entity.AspNetUsers> userRepository,
            IUserIdentity userIdentity
        ) : base(mapper, mediator, validator)
        {
            _canchaRepository = canchaRepository;
            _horarioCanchaRepository = horarioCanchaRepository;
            _reservaRepository = reservaRepository;
            _detalleReservaRepository = detalleReservaRepository;
            _pagoRepository = pagoRepository;
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

            // Agrupar TODOS los horarios en bloques consecutivos
            var horariosAgrupados = AgruparTodosLosHorariosConsecutivos(detalles);

            // Obtener pago activo
            var pagoActivo = await _pagoRepository.GetByAsNoTrackingAsync(
                p => p.IdReserva == reserva.IdReserva && p.Activo,
                p => p.IdEstadoPagoNavigation!
            );

            // Obtener información del operador que confirmó (si existe)
            string? nombreOperador = null;
            if (reserva.IdOperadorConfirmoNavigation != null)
            {
                nombreOperador = $"{reserva.IdOperadorConfirmoNavigation.IdUsuarioNavigation?.FirstName} {reserva.IdOperadorConfirmoNavigation.IdUsuarioNavigation?.LastName}".Trim();
            }

            // Obtener información de la cancha
            var cancha = reserva.IdCanchaNavigation;

            return new ReservaSlotDto
            {
                IdReserva = reserva.IdReserva,
                CodigoReserva = reserva.CodigoReserva ?? string.Empty,

                // Cliente
                Cliente = new ClienteSlotDto
                {
                    IdCliente = reserva.IdCliente.ToString(),
                    Nombre = $"{reserva.IdClienteNavigation.FirstName} {reserva.IdClienteNavigation.LastName}".Trim(),
                    Telefono = reserva.IdClienteNavigation.PhoneNumber ?? "",
                    Email = reserva.IdClienteNavigation.Email ?? ""
                },

                // Información básica
                Deporte = reserva.IdTipoDeporteNavigation?.Nombre ?? "",
                CantidadHoras = detalles.Count * 0.5m,  // Bloques de 30min
                Monto = reserva.MontoTotal,
                FechaExpiracion = reserva.FechaExpiracionPreReserva?.DateTime,

                // Estados de reserva
                EstadoReserva = reserva.IdEstadoReservaNavigation?.Nombre ?? "Desconocido",
                CodigoEstadoReserva = reserva.IdEstadoReservaNavigation?.Codigo ?? "",

                // Estados de pago
                EstadoPago = pagoActivo?.IdEstadoPagoNavigation?.Nombre ?? "Desconocido",
                CodigoEstadoPago = pagoActivo?.IdEstadoPagoNavigation?.Codigo ?? "",

                // Información de pago
                MontoAdelanto = pagoActivo?.MontoAdelanto ?? 0,
                MontoPendiente = pagoActivo?.MontoPendiente ?? reserva.MontoTotal,
                NumeroRecibo = pagoActivo?.NumeroReferencia,

                // Información adicional
                NombreOperadorConfirmo = nombreOperador,
                Observaciones = reserva.Observaciones,

                // Información de la cancha
                NombreCancha = cancha?.Nombre ?? "",
                DireccionCancha = cancha?.Direccion,
                TelefonoCancha = cancha?.TelefonoCancha,

                // Horarios agrupados en bloques consecutivos
                Horarios = horariosAgrupados
            };
        }

        /// <summary>
        /// Agrupa TODOS los horarios de una reserva en bloques consecutivos
        /// Devuelve una lista de HorarioDetalleDto para mostrar en el modal
        /// </summary>
        private List<HorarioDetalleDto> AgruparTodosLosHorariosConsecutivos(List<Entity.DetalleReserva> detalles)
        {
            var horariosAgrupados = new List<HorarioDetalleDto>();

            if (!detalles.Any())
            {
                return horariosAgrupados;
            }

            // Iniciar el primer bloque
            var bloqueActual = detalles.First();
            var horaInicioBloque = bloqueActual.IdHorarioCanchaNavigation!.IdHoraInicioNavigation!.Hora1;
            var horaFinBloque = bloqueActual.IdHorarioCanchaNavigation!.IdHoraFinNavigation!.Hora1;

            for (int i = 1; i < detalles.Count; i++)
            {
                var detalleActual = detalles[i];
                var detalleAnterior = detalles[i - 1];

                // Verificar si son consecutivos
                var idHoraFinAnterior = detalleAnterior.IdHorarioCanchaNavigation!.IdHoraFin;
                var idHoraInicioActual = detalleActual.IdHorarioCanchaNavigation!.IdHoraInicio;

                if (idHoraFinAnterior == idHoraInicioActual)
                {
                    // Extender el bloque actual
                    horaFinBloque = detalleActual.IdHorarioCanchaNavigation!.IdHoraFinNavigation!.Hora1;
                }
                else
                {
                    // Guardar el bloque actual y comenzar uno nuevo
                    horariosAgrupados.Add(new HorarioDetalleDto
                    {
                        HoraInicio = horaInicioBloque.ToString("HH:mm"),
                        HoraFin = horaFinBloque.ToString("HH:mm"),
                        HorarioFormateado = FormatearRangoHorario(horaInicioBloque, horaFinBloque)
                    });

                    // Iniciar nuevo bloque
                    horaInicioBloque = detalleActual.IdHorarioCanchaNavigation!.IdHoraInicioNavigation!.Hora1;
                    horaFinBloque = detalleActual.IdHorarioCanchaNavigation!.IdHoraFinNavigation!.Hora1;
                }
            }

            // Agregar el último bloque
            horariosAgrupados.Add(new HorarioDetalleDto
            {
                HoraInicio = horaInicioBloque.ToString("HH:mm"),
                HoraFin = horaFinBloque.ToString("HH:mm"),
                HorarioFormateado = FormatearRangoHorario(horaInicioBloque, horaFinBloque)
            });

            return horariosAgrupados;
        }

        /// <summary>
        /// Formatea un rango de horarios con la duración
        /// Ejemplo: "09:00 - 10:30 (1.5 horas)"
        /// </summary>
        private string FormatearRangoHorario(TimeOnly horaInicio, TimeOnly horaFin)
        {
            var duracion = (horaFin - horaInicio).TotalHours;
            var duracionTexto = duracion == 1 ? "1 hora" : $"{duracion:0.#} horas";
            return $"{horaInicio:HH\\:mm} - {horaFin:HH\\:mm} ({duracionTexto})";
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
