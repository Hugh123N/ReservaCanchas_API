using AutoMapper;
using MediatR;
using Reserva.Common;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Reserva;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Reserva
{
    /// <summary>
    /// Handler para obtener todas las reservas de un cliente
    /// </summary>
    public class ReservasClienteQueryHandler : IRequestHandler<ReservasClienteQuery, ResponseDto<IEnumerable<ReservaClienteDto>>>
    {
        private readonly IRepository<Entity.Reserva> _reservaRepository;
        private readonly IMapper _mapper;

        public ReservasClienteQueryHandler(
            IRepository<Entity.Reserva> reservaRepository,
            IMapper mapper)
        {
            _reservaRepository = reservaRepository;
            _mapper = mapper;
        }

        public async Task<ResponseDto<IEnumerable<ReservaClienteDto>>> Handle(
            ReservasClienteQuery request,
            CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ReservaClienteDto>>();

            try
            {
                var reservas = await _reservaRepository.FindByAsNoTrackingAsync(
                    r => r.IdUsuario == request.IdUsuario && r.Activo,
                    r => r.IdCanchaNavigation,
                    r => r.IdEstadoReservaNavigation,
                    r => r.ReservaDetalle,
                    r => r.Pago
                );

                var reservasDtos = reservas
                    .OrderByDescending(r => r.Fecha)
                    .ThenByDescending(r => r.CreateDate)
                    .Select(r => new ReservaClienteDto
                    {
                        IdReserva = r.IdReserva,
                        CodigoReserva = r.CodigoReserva,
                        Fecha = r.Fecha,
                        Monto = r.Monto ?? 0,

                        // Estado
                        EstadoReserva = r.IdEstadoReservaNavigation.Nombre,
                        CodigoEstadoReserva = r.IdEstadoReservaNavigation.Codigo!,

                        // Cancha
                        IdCancha = r.IdCancha,
                        NombreCancha = r.IdCanchaNavigation.Nombre,
                        DireccionCancha = r.IdCanchaNavigation.Direccion,
                        TelefonoCancha = r.IdCanchaNavigation.TelefonoCancha,

                        // Horarios
                        Horarios = r.ReservaDetalle
                            .OrderBy(d => d.HoraInicio)
                            .Select(d => new HorarioReservadoDto
                            {
                                HoraInicio = d.HoraInicio,
                                HoraFin = d.HoraFin
                            })
                            .ToList(),

                        // Pago
                        EstadoPago = r.Pago.FirstOrDefault(p => p.Activo)?.IdEstadoPagoNavigation?.Nombre ?? "Desconocido",
                        MontoAdelanto = r.Pago.FirstOrDefault(p => p.Activo)?.MontoAdelanto ?? 0,
                        MontoPendiente = r.Pago.FirstOrDefault(p => p.Activo)?.MontoPendiente ?? 0,
                        NumeroRecibo = r.Pago.FirstOrDefault(p => p.Activo)?.NumeroReferencia,

                        // Fechas
                        FechaExpiracionPreReserva = r.FechaExpiracionPreReserva,
                        FechaCreacion = r.CreateDate
                    })
                    .ToList();

                response.UpdateData(reservasDtos);
                response.AddOkResult($"Se encontraron {reservasDtos.Count} reservas.");
            }
            catch (Exception ex)
            {
                response.AddErrorResult($"Error al obtener las reservas: {ex.Message}");
            }

            return response;
        }
    }
}
