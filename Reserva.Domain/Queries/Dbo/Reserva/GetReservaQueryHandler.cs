using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Reserva;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Reserva
{
    /// <summary>
    /// Handler para obtener información completa de una reserva por ID
    /// </summary>
    public class GetReservaQueryHandler : QueryHandlerBase<GetReservaQuery, GetReservaDto>
    {
        private readonly IRepository<Entity.Reserva> _reservaRepository;
        private readonly IRepository<Entity.DetalleReserva> _detalleReservaRepository;
        private readonly IRepository<Entity.Pago> _pagoRepository;

        public GetReservaQueryHandler(
            IMapper mapper,
            GetReservaQueryValidator validator,
            IRepository<Entity.Reserva> reservaRepository,
            IRepository<Entity.DetalleReserva> detalleReservaRepository,
            IRepository<Entity.Pago> pagoRepository
        ) : base(mapper, validator)
        {
            _reservaRepository = reservaRepository;
            _detalleReservaRepository = detalleReservaRepository;
            _pagoRepository = pagoRepository;
        }

        protected override async Task<ResponseDto<GetReservaDto>> HandleQuery(GetReservaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetReservaDto>();

            // Obtener la reserva con todas las navegaciones necesarias
            var reserva = await _reservaRepository.GetByAsNoTrackingAsync(
                x => x.IdReserva == request.Id && x.Activo,
                r => r.IdTipoDeporteNavigation!,
                r => r.IdClienteNavigation!,
                r => r.IdEstadoReservaNavigation!,
                r => r.IdOperadorConfirmoNavigation!.IdUsuarioNavigation
            );

            if (reserva == null)
            {
                response.AddErrorResult("No se encontró la reserva especificada.");
                return response;
            }

            // Obtener detalles de la reserva con horarios
            var detallesResult = await _detalleReservaRepository.FindByAsNoTrackingAsync(
                d => d.IdReserva == reserva.IdReserva && d.Activo,
                d => d.IdHorarioCanchaNavigation!.IdHoraInicioNavigation!,
                d => d.IdHorarioCanchaNavigation!.IdHoraFinNavigation!
            );

            var detalles = detallesResult.ToList();

            // Agrupar horarios en bloques consecutivos
            var horarios = AgruparHorariosConsecutivos(detalles);

            var pagoActivo = await _pagoRepository.GetByAsNoTrackingAsync(
                p => p.IdReserva == reserva.IdReserva && p.Activo,
                p => p.IdEstadoPagoNavigation!
            );

            // Construir el DTO con información completa
            var reservaDto = new GetReservaDto
            {
                // Información base de la reserva
                IdReserva = reserva.IdReserva,
                IdCliente = reserva.IdCliente,
                CodigoReserva = reserva.CodigoReserva,
                IdCancha = reserva.IdCancha,
                IdTipoDeporte = reserva.IdTipoDeporte,
                FechaReserva = reserva.FechaReserva,
                MontoTotal = reserva.MontoTotal,
                IdEstadoReserva = reserva.IdEstadoReserva,
                FechaExpiracionPreReserva = reserva.FechaExpiracionPreReserva,
                IdOperadorConfirmo = reserva.IdOperadorConfirmo,
                FechaConfirmacion = reserva.FechaConfirmacion,
                Observaciones = reserva.Observaciones,
                Activo = reserva.Activo,

                // Horarios reservados
                Horarios = horarios,

                // Información de la cancha
                NombreCancha = reserva.IdCanchaNavigation?.Nombre ?? string.Empty,
                DireccionCancha = reserva.IdCanchaNavigation?.Direccion ?? string.Empty,
                TelefonoCancha = reserva.IdCanchaNavigation?.TelefonoCancha,

                // Información del cliente
                NombreCliente = reserva.IdClienteNavigation?.FirstName + reserva.IdClienteNavigation?.LastName ?? string.Empty,
                NumeroCliente = reserva.IdClienteNavigation?.PhoneNumber,

                // Estado Reserva
                EstadoReserva = reserva.IdEstadoReservaNavigation?.Nombre ?? "Desconocido",
                CodigoEstadoReserva = reserva.IdEstadoReservaNavigation?.Codigo ?? string.Empty,

                // Estado Pago
                EstadoPago = pagoActivo?.IdEstadoPagoNavigation?.Nombre ?? "Desconocido",
                CodigoEstadoPago = pagoActivo?.IdEstadoPagoNavigation?.Codigo ?? string.Empty,

                // Operador que confirmó (si existe)
                NombreOperadorConfirmo = reserva.IdOperadorConfirmoNavigation?.IdUsuarioNavigation.FirstName + reserva.IdOperadorConfirmoNavigation?.IdUsuarioNavigation.LastName,

                // Tipo de Deporte
                NombreDeporte = reserva.IdTipoDeporteNavigation?.Nombre,

                // Fechas de auditoría
                FechaCreacion = reserva.CreateDate,
                FechaModificacion = reserva.UpdateDate
            };

            response.UpdateData(reservaDto);
            response.AddOkResult("Reserva obtenida exitosamente.");

            return response;
        }

        /// <summary>
        /// Agrupa los horarios de una reserva en bloques consecutivos
        /// Ejemplo: [09:00-09:30, 09:30-10:00, 10:00-10:30, 14:00-14:30, 14:30-15:00]
        /// Resultado: [09:00-10:30 (1.5h), 14:00-15:00 (1h)]
        /// </summary>
        private List<HorarioReservadoDto> AgruparHorariosConsecutivos(List<Entity.DetalleReserva> detalles)
        {
            var horariosAgrupados = new List<HorarioReservadoDto>();

            if (!detalles.Any())
            {
                return horariosAgrupados;
            }

            // Ordenar por hora de inicio
            var detallesOrdenados = detalles
                .Where(d => d.IdHorarioCanchaNavigation?.IdHoraInicioNavigation != null
                            && d.IdHorarioCanchaNavigation?.IdHoraFinNavigation != null)
                .OrderBy(d => d.IdHorarioCanchaNavigation!.IdHoraInicio)
                .ToList();

            if (!detallesOrdenados.Any())
            {
                return horariosAgrupados;
            }

            // Iniciar el primer bloque
            var bloqueActual = detallesOrdenados.First();
            var horaInicioBloque = bloqueActual.IdHorarioCanchaNavigation!.IdHoraInicioNavigation!.Hora1;
            var horaFinBloque = bloqueActual.IdHorarioCanchaNavigation!.IdHoraFinNavigation!.Hora1;

            for (int i = 1; i < detallesOrdenados.Count; i++)
            {
                var detalleActual = detallesOrdenados[i];
                var detalleAnterior = detallesOrdenados[i - 1];

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
                    horariosAgrupados.Add(new HorarioReservadoDto
                    {
                        HoraInicio = horaInicioBloque,
                        HoraFin = horaFinBloque,
                        HorarioFormateado = FormatearRangoHorario(horaInicioBloque, horaFinBloque)
                    });

                    // Iniciar nuevo bloque
                    horaInicioBloque = detalleActual.IdHorarioCanchaNavigation!.IdHoraInicioNavigation!.Hora1;
                    horaFinBloque = detalleActual.IdHorarioCanchaNavigation!.IdHoraFinNavigation!.Hora1;
                }
            }

            // Agregar el último bloque
            horariosAgrupados.Add(new HorarioReservadoDto
            {
                HoraInicio = horaInicioBloque,
                HoraFin = horaFinBloque,
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
    }
}
