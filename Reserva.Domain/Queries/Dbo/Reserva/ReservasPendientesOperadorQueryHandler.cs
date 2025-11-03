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

        public ReservasPendientesOperadorQueryHandler(
            IMapper mapper,
            IRepository<Entity.Reserva> ReservaRepository
        ) : base(mapper)
        {
            _ReservaRepository = ReservaRepository;
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
                r => r.IdUsuarioNavigation,
                r => r.ReservaDetalle,
                r => r.IdEstadoReservaNavigation
            );

            // Mapear a DTOs con información completa
            var reservasDtos = reservasPendientes
                .OrderBy(r => r.FechaExpiracionPreReserva) // Más urgentes primero
                .Select(r => new ReservaPendienteOperadorDto
                {
                    IdReserva = r.IdReserva,
                    CodigoReserva = r.CodigoReserva,
                    Fecha = r.Fecha,
                    HoraInicio = r.ReservaDetalle.Any()
                        ? r.ReservaDetalle.Min(d => d.HoraInicio).ToString("HH:mm")
                        : null,
                    HoraFin = r.ReservaDetalle.Any()
                        ? r.ReservaDetalle.Max(d => d.HoraFin).ToString("HH:mm")
                        : null,
                    Monto = r.Monto,
                    FechaCreacion = r.CreateDate,
                    FechaExpiracion = r.FechaExpiracionPreReserva,
                    HorasRestantes = r.FechaExpiracionPreReserva.HasValue
                        ? (r.FechaExpiracionPreReserva.Value - DateTimeOffset.Now).TotalHours
                        : 0,
                    NombreCancha = r.IdCanchaNavigation.Nombre,
                    IdCliente = r.IdUsuario,
                    NombreCliente = r.IdUsuarioNavigation.UserName,
                    EmailCliente = r.IdUsuarioNavigation.Email,
                    TelefonoCliente = r.IdUsuarioNavigation.PhoneNumber,
                    NivelUrgencia = CalcularNivelUrgencia(r.FechaExpiracionPreReserva)
                })
                .ToList();

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
