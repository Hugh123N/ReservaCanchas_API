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

            if (metodoPago == null)
            {
                response.AddErrorResult("El método de pago seleccionado no existe.");
                return response;
            }

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

            //Crear el pago con estado Pendiente
            var estadoPendientePago = await _EstadoPagoRepository.GetByAsNoTrackingAsync(
                x => x.Codigo!.Equals(Constants.ESTADO_PAGO.Pendiente));

            if (estadoPendientePago == null)
            {
                response.AddErrorResult("Error del sistema: Estado de pago 'Pendiente' no encontrado.");
                return response;
            }

            var nuevoPago = new Entity.Pago
            {
                IdReserva = nuevaReserva.IdReserva,
                Moneda = "PEN",
                Monto = request.CreateDto.Monto ?? 0,
                IdMetodoPago = metodoPago.IdMetodoPago,
                IdEstadoPago = estadoPendientePago.IdEstadoPago,
                CodigoOperacion = null 
            };

            await _PagoRepository.AddAsync(nuevoPago);
            await _PagoRepository.SaveAsync();

            // 7. Procesar el pago usando la estrategia correspondiente (Strategy Pattern)
            var estrategiaPago = _pagoStrategyFactory.ObtenerEstrategia(metodoPago.Codigo!);
            var resultadoPago = await estrategiaPago.ProcesarPagoAsync(nuevoPago, cancha, nuevaReserva);

            // 8. Configuración de expiración
            int minutosExpiracion = _configuration.GetValue<int>("Pago:MinutosExpiracion", 15);

            // 9. Mapear y construir respuesta
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