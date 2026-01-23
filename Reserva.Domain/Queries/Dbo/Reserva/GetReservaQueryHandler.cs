using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Reserva;
using Reserva.Dto.Dbo.DetalleReserva;
using Reserva.Dto.Dbo.Pago;
using Reserva.Dto.Dbo.ConfiguracionProveedor;
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
                r => r.IdOperadorConfirmoNavigation!.IdUsuarioNavigation,
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

            var horariosDto = _mapper?.Map<List<GetDetalleReservaDto>>(detalles) ?? new List<GetDetalleReservaDto>();
            var pagoDto = _mapper?.Map<GetPagoDto>(pagoActivo);
            var configuracionDto = _mapper?.Map<GetConfiguracionProveedorDto>(configuracionProveedor);
            var reservaDto = _mapper?.Map<GetReservaDto>(reserva);

            reservaDto.CodigoReserva = reserva.CodigoReserva;

            reservaDto.NombreCancha = reserva.IdCanchaNavigation?.Nombre ?? string.Empty;
            reservaDto.DireccionCancha = reserva.IdCanchaNavigation?.Direccion ?? string.Empty;
            reservaDto.TelefonoCancha = reserva.IdCanchaNavigation?.TelefonoCancha;

            reservaDto.NombreCliente = reserva.IdClienteNavigation?.FirstName + " " + reserva.IdClienteNavigation?.LastName ?? string.Empty;
            reservaDto.NumeroCliente = reserva.IdClienteNavigation?.PhoneNumber;
            reservaDto.EmailCliente = reserva.IdClienteNavigation?.Email;

            reservaDto.NombreOperadorConfirmo = reserva.IdOperadorConfirmoNavigation?.IdUsuarioNavigation != null
                ? reserva.IdOperadorConfirmoNavigation.IdUsuarioNavigation.FirstName + " " + reserva.IdOperadorConfirmoNavigation.IdUsuarioNavigation.LastName
                : null; // TODO: en caso no tengo operador y solo eata encargado el proveedor deberia pasar el proveedor

            reservaDto.NombreDeporte = reserva.IdTipoDeporteNavigation?.Nombre; //TODO SE PUEDE RESERVAR VARIOS DEPORTES EN UNA SOLO RESERVA
            reservaDto.FechaCreacion = reserva.CreateDate;
            reservaDto.FechaModificacion = reserva.UpdateDate;

            reservaDto.ConfiguracionProveedor = configuracionDto;
            reservaDto.Horarios = horariosDto;
            reservaDto.Pago = pagoDto;

            response.UpdateData(reservaDto);
            response.AddOkResult("Reserva obtenida exitosamente.");

            return response;
        }
    }
}
