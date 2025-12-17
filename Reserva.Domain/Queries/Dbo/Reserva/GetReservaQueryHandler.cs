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
            var detalles = await _detalleReservaRepository.FindByAsNoTrackingAsync(
                d => d.IdReserva == reserva.IdReserva && d.Activo,
                d => d.IdHorarioCanchaNavigation!.IdHoraInicioNavigation!,
                d => d.IdHorarioCanchaNavigation!.IdHoraFinNavigation!
            );

            // Mapear horarios
            var horarios = detalles
                .Where(d => d.IdHorarioCanchaNavigation?.IdHoraInicioNavigation != null
                            && d.IdHorarioCanchaNavigation?.IdHoraFinNavigation != null)
                .Select(d => new HorarioReservadoDto
                {
                    HoraInicio = d.IdHorarioCanchaNavigation!.IdHoraInicioNavigation!.Hora1,
                    HoraFin = d.IdHorarioCanchaNavigation!.IdHoraFinNavigation!.Hora1
                })
                .OrderBy(h => h.HoraInicio)
                .ToList();

            // Obtener información del pago activo con su estado
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
                NombreOperador = reserva.IdOperadorConfirmoNavigation?.IdUsuarioNavigation.FirstName + reserva.IdOperadorConfirmoNavigation?.IdUsuarioNavigation.LastName,

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
    }
}
