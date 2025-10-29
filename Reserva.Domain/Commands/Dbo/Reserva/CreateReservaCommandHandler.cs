using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Services.Pago;
using Reserva.Dto.Dbo.Pago;
using Reserva.Dto.Dbo.Reserva;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Reserva
{
    public class CreateReservaCommandHandler : CommandHandlerBase<CreateReservaCommand, ReservaConPagoDto>
    {
        private readonly IRepository<Entity.Reserva> _ReservaRepository;
        private readonly IRepository<Entity.Pago> _PagoRepository;
        private readonly IRepository<Entity.EstadoReserva> _EstadoReservaRepository;
        private readonly IRepository<Entity.EstadoPago> _EstadoPagoRepository;
        private readonly IRepository<Entity.Cancha> _CanchaRepository;
        private readonly IRepository<Entity.MetodoPago> _MetodoPagoRepository;
        private readonly IConfiguration _configuration;
        private readonly PagoStrategyFactory _pagoStrategyFactory;

        public CreateReservaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateReservaCommandValidator validator,
            IRepository<Entity.Reserva> ReservaRepository,
            IRepository<Entity.Pago> PagoRepository,
            IRepository<Entity.EstadoReserva> EstadoReservaRepository,
            IRepository<Entity.EstadoPago> EstadoPagoRepository,
            IRepository<Entity.Cancha> CanchaRepository,
            IRepository<Entity.MetodoPago> MetodoPagoRepository,
            IConfiguration configuration
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _ReservaRepository = ReservaRepository;
            _PagoRepository = PagoRepository;
            _EstadoReservaRepository = EstadoReservaRepository;
            _EstadoPagoRepository = EstadoPagoRepository;
            _CanchaRepository = CanchaRepository;
            _MetodoPagoRepository = MetodoPagoRepository;
            _configuration = configuration;
            _pagoStrategyFactory = new PagoStrategyFactory(configuration);
        }

        public override async Task<ResponseDto<ReservaConPagoDto>> HandleCommand(CreateReservaCommand request,CancellationToken cancellationToken)
        {
            var response = new ResponseDto<ReservaConPagoDto>();

            var cancha = await _CanchaRepository.GetByAsync(c => c.IdCancha == request.CreateDto.IdCancha,
                c => c.IdProveedorNavigation!,
                c => c.IdProveedorNavigation!.IdProveedorNavigation);

            var metodoPago = await _MetodoPagoRepository.GetByAsync(mp => mp.Codigo == request.CreateDto.CodigoMetodoPago);

            if (!_pagoStrategyFactory.EsMetodoPagoSoportado(metodoPago.Codigo!))
            {
                response.AddErrorResult($"El método de pago '{metodoPago.Nombre}' no está soportado actualmente.");
                return response;
            }

            //Validar disponibilidad: No debe haber otra reserva activa en ese horario
            var fechaReserva = DateOnly.FromDateTime(request.CreateDto.Fecha);
            var horaInicio = TimeOnly.FromTimeSpan(request.CreateDto.HoraInicio);
            var horaFin = TimeOnly.FromTimeSpan(request.CreateDto.HoraFin);

            var reservaConflicto = await _ReservaRepository.GetByAsNoTrackingAsync(r => r.IdCancha == request.CreateDto.IdCancha &&
                     r.Fecha == fechaReserva && r.Activo &&
                     r.IdEstadoReservaNavigation.Codigo != Constants.ESTADO_RESERVA.Cancelado &&
                     ((r.HoraInicio < horaFin && r.HoraFin > horaInicio)),
                r => r.IdEstadoReservaNavigation
            );

            if (reservaConflicto != null)
            {
                response.AddErrorResult($"Ya existe una reserva para esta cancha en el horario seleccionado ({reservaConflicto.HoraInicio:hh\\:mm} - {reservaConflicto.HoraFin:hh\\:mm}).");
                return response;
            }

            var estadoPendienteReserva = await _EstadoReservaRepository.GetByAsNoTrackingAsync(
                x => x.Codigo!.Equals(Constants.ESTADO_RESERVA.Pendiente));

            var nuevaReserva = _mapper?.Map<Entity.Reserva>(request.CreateDto);
            if (nuevaReserva == null)
            {
                response.AddErrorResult("Error al mapear los datos de la reserva.");
                return response;
            }

            nuevaReserva.IdEstadoReserva = estadoPendienteReserva.IdEstadoReserva;
            nuevaReserva.Fecha = fechaReserva;
            nuevaReserva.HoraInicio = horaInicio;
            nuevaReserva.HoraFin = horaFin;

            await _ReservaRepository.AddAsync(nuevaReserva);
            await _ReservaRepository.SaveAsync();

            // 6. Crear el pago - manejar adelantos para EFECTIVO
            decimal montoAdelanto = request.CreateDto.MontoAdelanto ?? 0;
            decimal montoTotal = request.CreateDto.Monto ?? 0;
            decimal montoPendiente = montoTotal - montoAdelanto;

            // Validar adelanto solo para EFECTIVO
            if (montoAdelanto > 0 && metodoPago.Codigo != Constants.METODO_PAGO.Efectivo)
            {
                response.AddErrorResult($"Solo se permiten adelantos para pagos en efectivo. Método seleccionado: {metodoPago.Nombre}");
                return response;
            }

            // Determinar estado del pago
            var  estadoPago = new Entity.EstadoPago();

            if (montoAdelanto >= montoTotal)
            {
                // Pago completo desde el inicio
                estadoPago = await _EstadoPagoRepository.GetByAsNoTrackingAsync(x => x.Codigo!.Equals(Constants.ESTADO_PAGO.Pagado));
                montoAdelanto = montoTotal;
                montoPendiente = 0;
            }
            else if (montoAdelanto > 0)
            {
                // Pago parcial
                estadoPago = await _EstadoPagoRepository.GetByAsNoTrackingAsync(
                    x => x.Codigo!.Equals(Constants.ESTADO_PAGO.Parcial));
            }
            else
            {
                // Sin adelanto
                estadoPago = await _EstadoPagoRepository.GetByAsNoTrackingAsync(
                    x => x.Codigo!.Equals(Constants.ESTADO_PAGO.Pendiente));
            }

            if (estadoPago == null)
            {
                response.AddErrorResult("Error del sistema: Estado de pago no encontrado.");
                return response;
            }

            var nuevoPago = new Entity.Pago
            {
                IdReserva = nuevaReserva.IdReserva,
                Moneda = "PEN",
                Monto = montoTotal,
                MontoAdelanto = montoAdelanto,
                MontoPendiente = montoPendiente,
                IdMetodoPago = metodoPago.IdMetodoPago,
                IdEstadoPago = estadoPago.IdEstadoPago,
                CodigoOperacion = null
            };

            await _PagoRepository.AddAsync(nuevoPago);
            await _PagoRepository.SaveAsync();

            // Si hay adelanto y cumple el porcentaje mínimo, confirmar reserva
            if (montoAdelanto > 0 && metodoPago.Codigo == Constants.METODO_PAGO.Efectivo)
            {
                decimal porcentajeAdelanto = (montoAdelanto / montoTotal) * 100;
                decimal porcentajeMinimoParaConfirmar = _configuration.GetValue<decimal>("Pago:PorcentajeMinimoAdelanto", 50);

                if (porcentajeAdelanto >= porcentajeMinimoParaConfirmar)
                {
                    var estadoConfirmado = await _EstadoReservaRepository.GetByAsNoTrackingAsync(
                        x => x.Codigo!.Equals(Constants.ESTADO_RESERVA.Confirmado));

                    if (estadoConfirmado != null)
                    {
                        nuevaReserva.IdEstadoReserva = estadoConfirmado.IdEstadoReserva;
                        await _ReservaRepository.UpdateAsync(nuevaReserva);
                        await _ReservaRepository.SaveAsync();
                    }
                }
            }

            var estrategiaPago = _pagoStrategyFactory.ObtenerEstrategia(metodoPago.Codigo!);
            var resultadoPago = await estrategiaPago.ProcesarPagoAsync(nuevoPago, cancha, nuevaReserva);

            int minutosExpiracion = _configuration.GetValue<int>("Pago:MinutosExpiracion", 15);

            var reservaDto = _mapper?.Map<GetReservaDto>(nuevaReserva);
            var pagoDto = _mapper?.Map<GetPagoDto>(nuevoPago);

            var reservaConPagoDto = new ReservaConPagoDto
            {
                Reserva = reservaDto!,
                Pago = pagoDto!,

                // Datos específicos del método de pago (Strategy Pattern)
                QrCodeBase64 = resultadoPago.QrCodeBase64,
                QrText = resultadoPago.QrText,
                NumeroCuenta = resultadoPago.NumeroCuenta,
                CCI = resultadoPago.CCI,
                NombreBanco = resultadoPago.NombreBanco,
                TitularCuenta = resultadoPago.TitularCuenta,
                InformacionAdicional = resultadoPago.InformacionAdicional,

                // Datos generales
                MetodoPago = metodoPago.Nombre,
                MontoFormateado = nuevoPago.Monto.ToString("F2"),
                Moneda = nuevoPago.Moneda,
                MinutosExpiracion = minutosExpiracion,
                FechaExpiracion = nuevoPago.CreateDate.AddMinutes(minutosExpiracion)
            };

            response.UpdateData(reservaConPagoDto);
            response.AddOkResult($"Reserva creada exitosamente con método de pago: {metodoPago.Nombre}.");

            return response;
        }
    }
}