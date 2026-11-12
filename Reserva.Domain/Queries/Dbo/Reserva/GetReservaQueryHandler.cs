using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Reserva;
using Reserva.Domain.Helpers;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Reserva
{
    public class GetReservaQueryHandler : QueryHandlerBase<GetReservaQuery, GetReservaDto>
    {
        private readonly IRepository<Entity.Reserva> _reservaRepository;
        private readonly IRepository<Entity.DetalleReserva> _detalleReservaRepository;
        private readonly IRepository<Entity.Pago> _pagoRepository;
        private readonly IRepository<Entity.ConfiguracionProveedor> _configuracionProveedorRepository;

        public GetReservaQueryHandler(
            IMapper mapper,
            GetReservaQueryValidator validator,
            IRepository<Entity.Reserva> reservaRepository,
            IRepository<Entity.DetalleReserva> detalleReservaRepository,
            IRepository<Entity.Pago> pagoRepository,
            IRepository<Entity.ConfiguracionProveedor> configuracionProveedorRepository
        ) : base(mapper, validator)
        {
            _reservaRepository = reservaRepository;
            _detalleReservaRepository = detalleReservaRepository;
            _pagoRepository = pagoRepository;
            _configuracionProveedorRepository = configuracionProveedorRepository;
        }

        protected override async Task<ResponseDto<GetReservaDto>> HandleQuery(GetReservaQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetReservaDto>();

            var reserva = await _reservaRepository.GetByAsNoTrackingAsync(
                x => x.IdReserva == request.Id && x.Activo,
                r => r.IdTipoDeporteNavigation!,
                r => r.IdClienteNavigation!,
                r => r.IdEstadoReservaNavigation!,
                r => r.IdOperadorConfirmoNavigation!,
                r => r.IdCanchaNavigation!
            );

            if (reserva == null)
            {
                response.AddErrorResult("No se encontró la reserva especificada.");
                return response;
            }

            var detallesResult = await _detalleReservaRepository.FindByAsNoTrackingAsync(
                d => d.IdReserva == reserva.IdReserva && d.Activo,
                d => d.IdHorarioCanchaNavigation!.IdHoraInicioNavigation!,
                d => d.IdHorarioCanchaNavigation!.IdHoraFinNavigation!
            );
            var detalles = detallesResult.ToList();

            var pagoActivo = await _pagoRepository.GetByAsNoTrackingAsync(
                p => p.IdReserva == reserva.IdReserva && p.Activo,
                p => p.IdEstadoPagoNavigation!
            );

            var configuracionProveedor = await _configuracionProveedorRepository.GetByAsNoTrackingAsync(
                c => c.IdProveedor == reserva.IdCanchaNavigation!.IdProveedor && c.Activo);

            var reservaDto = ReservaDetalleDtoMapper.Map(_mapper!, reserva, detalles, pagoActivo, configuracionProveedor);

            response.UpdateData(reservaDto);
            response.AddOkResult("Reserva obtenida exitosamente.");

            return response;
        }
    }
}
