using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Reserva;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Reserva
{
    public class ReservasPendientesOperadorQueryHandler : QueryHandlerBase<ReservasPendientesOperadorQuery, IEnumerable<ReservaPendienteOperadorDto>>
    {
        private readonly IRepository<Entity.Reserva> _ReservaRepository;
        private readonly IRepository<Entity.DetalleReserva> _DetalleReservaRepository;

        public ReservasPendientesOperadorQueryHandler(
            IMapper mapper,
            IRepository<Entity.Reserva> ReservaRepository,
            IRepository<Entity.DetalleReserva> DetalleReservaRepository
        ) : base(mapper)
        {
            _ReservaRepository = ReservaRepository;
            _DetalleReservaRepository = DetalleReservaRepository;
        }

        protected override async Task<ResponseDto<IEnumerable<ReservaPendienteOperadorDto>>> HandleQuery(
            ReservasPendientesOperadorQuery request,
            CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ReservaPendienteOperadorDto>>();

            // Obtener reservas pendientes de las canchas del proveedor
            var reservasPendientes = await _ReservaRepository.FindByAsNoTrackingAsync(
                r => r.IdCanchaNavigation.IdProveedor == request.IdProveedor
                     && r.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Pendiente
                     && r.Activo,
                r => r.IdCanchaNavigation,
                r => r.IdClienteNavigation,
                r => r.IdEstadoReservaNavigation
            );

            if (!reservasPendientes.Any())
            {
                response.UpdateData(new List<ReservaPendienteOperadorDto>());
                response.AddOkResult("No se encontraron reservas pendientes.");
                return await Task.FromResult(response);
            }

            var idsReservas = reservasPendientes.Select(r => r.IdReserva).ToList();

            var todosLosDetalles = await _DetalleReservaRepository.FindByAsNoTrackingAsync(
                d => idsReservas.Contains(d.IdReserva) && d.Activo,
                d => d.IdHorarioCanchaNavigation!.IdHoraInicioNavigation!,
                d => d.IdHorarioCanchaNavigation!.IdHoraFinNavigation!
            );

            // Agrupar detalles por IdReserva para acceso rápido
            var detallesPorReserva = todosLosDetalles
                .GroupBy(d => d.IdReserva)
                .ToDictionary(g => g.Key, g => g.ToList());

            var reservasDtos = new List<ReservaPendienteOperadorDto>();

            foreach (var r in reservasPendientes.OrderBy(r => r.FechaExpiracionPreReserva))
            {
                TimeOnly? horaInicio = null;
                TimeOnly? horaFin = null;

                if (detallesPorReserva.TryGetValue(r.IdReserva, out var detallesReserva))
                {
                    var horasValidas = detallesReserva
                        .Where(d => d.IdHorarioCanchaNavigation?.IdHoraInicioNavigation != null
                                 && d.IdHorarioCanchaNavigation?.IdHoraFinNavigation != null)
                        .ToList();

                    if (horasValidas.Any())
                    {
                        horaInicio = horasValidas.Min(d => d.IdHorarioCanchaNavigation!.IdHoraInicioNavigation!.Hora1);
                        horaFin = horasValidas.Max(d => d.IdHorarioCanchaNavigation!.IdHoraFinNavigation!.Hora1);
                    }
                }

                reservasDtos.Add(new ReservaPendienteOperadorDto
                {
                    IdReserva = r.IdReserva,
                    CodigoReserva = r.CodigoReserva,
                    Fecha = r.FechaReserva,
                    HoraInicio = horaInicio?.ToString("HH:mm"),
                    HoraFin = horaFin?.ToString("HH:mm"),
                    Monto = r.MontoTotal,
                    FechaCreacion = r.CreateDate,
                    FechaExpiracion = r.FechaExpiracionPreReserva,
                    HorasRestantes = r.FechaExpiracionPreReserva.HasValue
                        ? (r.FechaExpiracionPreReserva.Value - DateTimeOffset.Now).TotalHours
                        : 0,
                    NombreCancha = r.IdCanchaNavigation.Nombre,
                    IdCliente = r.IdCliente,
                    NombreCliente = r.IdClienteNavigation.UserName,
                    EmailCliente = r.IdClienteNavigation.Email,
                    TelefonoCliente = r.IdClienteNavigation.PhoneNumber,
                    NivelUrgencia = CalcularNivelUrgencia(r.FechaExpiracionPreReserva)
                });
            }

            response.UpdateData(reservasDtos);
            response.AddOkResult($"Se encontraron {reservasDtos.Count} reservas pendientes.");

            return await Task.FromResult(response);
        }

        /// <summary>
        /// Calcula el nivel de urgencia según el tiempo restante
        /// </summary>
        private string CalcularNivelUrgencia(DateTimeOffset? fechaExpiracion)
        {
            if (!fechaExpiracion.HasValue)
                return "BAJA";

            var horasRestantes = (fechaExpiracion.Value - DateTimeOffset.Now).TotalHours;

            if (horasRestantes < 0)
                return "EXPIRADA";
            else if (horasRestantes <= 6)
                return "CRÍTICA";
            else if (horasRestantes <= 24)
                return "ALTA";
            else
                return "MEDIA";
        }
    }
}
