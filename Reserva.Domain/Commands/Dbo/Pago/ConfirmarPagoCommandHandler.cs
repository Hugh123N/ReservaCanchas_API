using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Services.Pago;
using Reserva.Dto.Dbo.Pago;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Pago
{
    public class ConfirmarPagoCommandHandler : CommandHandlerBase<ConfirmarPagoCommand, GetPagoDto>
    {
        private readonly IRepository<Entity.Pago> _PagoRepository;
        private readonly IRepository<Entity.Reserva> _ReservaRepository;
        private readonly IRepository<Entity.EstadoPago> _EstadoPagoRepository;
        private readonly IRepository<Entity.EstadoReserva> _EstadoReservaRepository;
        private readonly IRepository<Entity.MetodoPago> _MetodoPagoRepository;
        private readonly IConfiguration _configuration;

        public ConfirmarPagoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            ConfirmarPagoCommandValidator validator,
            IRepository<Entity.Pago> PagoRepository,
            IRepository<Entity.Reserva> ReservaRepository,
            IRepository<Entity.EstadoPago> EstadoPagoRepository,
            IRepository<Entity.EstadoReserva> EstadoReservaRepository,
            IRepository<Entity.MetodoPago> MetodoPagoRepository,
            IConfiguration configuration
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _PagoRepository = PagoRepository;
            _ReservaRepository = ReservaRepository;
            _EstadoPagoRepository = EstadoPagoRepository;
            _EstadoReservaRepository = EstadoReservaRepository;
            _MetodoPagoRepository = MetodoPagoRepository;
            _configuration = configuration;
        }

        public override async Task<ResponseDto<GetPagoDto>> HandleCommand(
            ConfirmarPagoCommand request,
            CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetPagoDto>();

            var pago = await _PagoRepository.GetByAsync(p => p.IdPago == request.ConfirmarDto.IdPago,
                p => p.IdEstadoPagoNavigation,
                p => p.IdMetodoPagoNavigation!,
                p => p.IdReservaNavigation!
                );

            if (pago == null)
            {
                response.AddErrorResult("El pago no existe.");
                return response;
            }

            if (pago.IdEstadoPagoNavigation.Codigo != Constants.ESTADO_PAGO.Pendiente)
            {
                response.AddErrorResult($"El pago no está en estado Pendiente. Estado actual: {pago.IdEstadoPagoNavigation.Nombre}");
                return response;
            }

            //Validar que el pago no ha expirado
            int minutosExpiracion = _configuration.GetValue<int>("Pago:MinutosExpiracion", 15);
            var fechaExpiracion = pago.CreateDate.AddMinutes(minutosExpiracion);

            if (DateTimeOffset.UtcNow > fechaExpiracion)
            {
                response.AddErrorResult($"El tiempo para completar el pago ha expirado. Tiempo límite: {minutosExpiracion} minutos.");
                return response;
            }

            bool codigoValido = false;
            string codigoOperacion = request.ConfirmarDto.CodigoOperacion.ToUpper().Trim();
            var _qrCodeService = new QrCodeService();

            if (pago.IdMetodoPagoNavigation?.Codigo == Constants.METODO_PAGO.Yape)
            {
                codigoValido = _qrCodeService.ValidarCodigoOperacionYape(codigoOperacion);
            }
            else if (pago.IdMetodoPagoNavigation?.Codigo == Constants.METODO_PAGO.Plin)
            {
                codigoValido = _qrCodeService.ValidarCodigoOperacionPlin(codigoOperacion);
            }
            else
            {
                // Para otros métodos de pago, aceptar cualquier código válido
                codigoValido = !string.IsNullOrWhiteSpace(codigoOperacion) && codigoOperacion.Length >= 6;
            }

            if (!codigoValido)
            {
                response.AddErrorResult("El formato del código de operación no es válido.");
                return response;
            }

            var estadoPagado = await _EstadoPagoRepository.GetByAsNoTrackingAsync(x => x.Codigo!.Equals(Constants.ESTADO_PAGO.Pagado));

            if (estadoPagado == null)
            {
                response.AddErrorResult("Error del sistema: Estado de pago 'Pagado' no encontrado.");
                return response;
            }

            pago.CodigoOperacion = codigoOperacion;
            pago.IdEstadoPago = estadoPagado.IdEstadoPago;

            await _PagoRepository.UpdateAsync(pago);
            await _PagoRepository.SaveAsync();

            //Si el pago está asociado a una reserva, actualizar su estado a Confirmado
            if (pago.IdReserva.HasValue && pago.IdReservaNavigation != null)
            {
                var estadoConfirmado = await _EstadoReservaRepository.GetByAsNoTrackingAsync(x => x.Codigo!.Equals(Constants.ESTADO_RESERVA.Confirmado));

                if (estadoConfirmado != null)
                {
                    var reserva = pago.IdReservaNavigation;
                    reserva.IdEstadoReserva = estadoConfirmado.IdEstadoReserva;

                    await _ReservaRepository.UpdateAsync(reserva);
                    await _ReservaRepository.SaveAsync();
                }
            }

            var pagoDto = _mapper?.Map<GetPagoDto>(pago);
            if (pagoDto != null)
            {
                response.UpdateData(pagoDto);
            }

            response.AddOkResult($"Pago confirmado exitosamente. Código de operación: {codigoOperacion}");

            return response;
        }
    }
}
