using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Services.Notificacion;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Pago;
using Reserva.Dto.Dbo.Reserva;
using Reserva.Entity;
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
        private readonly IRepository<Entity.Operador> _OperadorRepository;
        private readonly IConfiguration _configuration;
        private readonly INotificacionService _notificacionService;

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
            IRepository<Entity.Operador> OperadorRepository,
            IConfiguration configuration,
            INotificacionService notificacionService
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _ReservaRepository = ReservaRepository;
            _PagoRepository = PagoRepository;
            _EstadoReservaRepository = EstadoReservaRepository;
            _EstadoPagoRepository = EstadoPagoRepository;
            _CanchaRepository = CanchaRepository;
            _MetodoPagoRepository = MetodoPagoRepository;
            _OperadorRepository = OperadorRepository;
            _configuration = configuration;
            _notificacionService = notificacionService;
        }

        public override async Task<ResponseDto<ReservaConPagoDto>> HandleCommand(CreateReservaCommand request,CancellationToken cancellationToken)
        {
            var response = new ResponseDto<ReservaConPagoDto>();

            var cancha = await _CanchaRepository.GetByAsync(
                c => c.IdCancha == request.CreateDto.IdCancha,
                c => c.IdProveedorNavigation!,
                c => c.IdProveedorNavigation!,
                c => c.IdProveedorNavigation!.IdUsuarioNavigation,
                c => c.OperadorCancha);


            if (cancha == null)
            {
                response.AddErrorResult("La cancha seleccionada no existe.");
                return response;
            }

            var metodoPago = await _MetodoPagoRepository.GetByAsync(mp => mp.Codigo == request.CreateDto.CodigoMetodoPago);

            if (metodoPago == null)
            {
                response.AddErrorResult("El método de pago seleccionado no es válido.");
                return response;
            }

            if (metodoPago.Codigo != Constants.METODO_PAGO.Efectivo)
            {
                response.AddErrorResult($"Solo se acepta pago en efectivo. El método '{metodoPago.Nombre}' no está disponible para reservas.");
                return response;
            }

            //Validar disponibilidad de horario
            var reservasDelDia = await _ReservaRepository.FindByAsNoTrackingAsync(
                r => r.IdCancha == request.CreateDto.IdCancha
                     && r.Fecha.Date == request.CreateDto.Fecha.Date
                     && r.Activo
                     && r.IdEstadoReservaNavigation.Codigo != Constants.ESTADO_RESERVA.Cancelado
                     && r.IdEstadoReservaNavigation.Codigo != Constants.ESTADO_RESERVA.Expirado,
                r => r.ReservaDetalle
            );

            foreach (var detalle in request.CreateDto.Detalles)
            {
                var existeConflicto = reservasDelDia.Any(r =>
                    r.ReservaDetalle.Any(d => d.HoraInicio < detalle.HoraFin &&
                        d.HoraFin > detalle.HoraInicio
                    )
                );

                if (existeConflicto)
                {
                    response.AddErrorResult(
                        $"La hora {detalle.HoraInicio:hh\\:mm} - {detalle.HoraFin:hh\\:mm} ya está reservada.");
                    return response;
                }
            }

            var estadoPendienteReserva = await _EstadoReservaRepository.GetByAsNoTrackingAsync(
                x => x.Codigo!.Equals(Constants.ESTADO_RESERVA.Pendiente));

            if (estadoPendienteReserva == null)
            {
                response.AddErrorResult("Error del sistema: Estado de reserva no encontrado.");
                return response;
            }

            var nuevaReserva = _mapper?.Map<Entity.Reserva>(request.CreateDto);
            if (nuevaReserva == null)
            {
                response.AddErrorResult("Error al mapear los datos de la reserva.");
                return response;
            }

            nuevaReserva.ReservaDetalle = request.CreateDto.Detalles.Select(x => new ReservaDetalle
            {
                HoraInicio = x.HoraInicio,
                HoraFin = x.HoraFin
            }).ToList();

            int duracionPreReservaHoras = cancha.DuracionPreReserva ?? 24;
            nuevaReserva.FechaExpiracionPreReserva = DateTimeOffset.Now.AddHours(duracionPreReservaHoras);
            nuevaReserva.CodigoReserva = await GenerarCodigoReserva();
            nuevaReserva.IdEstadoReserva = estadoPendienteReserva.IdEstadoReserva;
            nuevaReserva.IdCanchaNavigation = null;
            nuevaReserva.RecordatorioEnviado = false;
            nuevaReserva.NotificacionAdvertenciaEnviada = false;

            await _ReservaRepository.AddAsync(nuevaReserva);
            await _ReservaRepository.SaveAsync();

            decimal montoTotal = request.CreateDto.Monto ?? 0;

            var estadoPagoPendiente = await _EstadoPagoRepository.GetByAsNoTrackingAsync(
                x => x.Codigo!.Equals(Constants.ESTADO_PAGO.Pendiente));

            if (estadoPagoPendiente == null)
            {
                response.AddErrorResult("Error del sistema: Estado de pago no encontrado.");
                return response;
            }

            var nuevoPago = new Entity.Pago
            {
                IdReserva = nuevaReserva.IdReserva,
                Moneda = "PEN",
                Monto = montoTotal,
                MontoAdelanto = 0,
                MontoPendiente = montoTotal,
                IdMetodoPago = metodoPago.IdMetodoPago,
                IdEstadoPago = estadoPagoPendiente.IdEstadoPago,
                CodigoOperacion = null
            };

            await _PagoRepository.AddAsync(nuevoPago);
            await _PagoRepository.SaveAsync();

            var reservaDto = _mapper?.Map<GetReservaDto>(nuevaReserva);
            var pagoDto = _mapper?.Map<GetPagoDto>(nuevoPago);

            var operadores = await _OperadorRepository.FindByAsNoTrackingAsync(x => x.OperadorCancha.Any(c => c.IdCancha == cancha.IdCancha),
                x => x.IdUsuarioNavigation
            );

            var nombresOperadores = string.Join(", ",
                operadores.Select(o => o.IdUsuarioNavigation.FirstName + " " + o.IdUsuarioNavigation.LastName)
            );

            var reservaConPagoDto = new ReservaConPagoDto
            {
                Reserva = reservaDto!,
                Pago = pagoDto!,

                TelefonoCancha = cancha.TelefonoCancha ?? cancha.IdProveedorNavigation.IdUsuarioNavigation.PhoneNumber,
                NombreOperador = !string.IsNullOrEmpty(nombresOperadores) ? nombresOperadores : cancha.IdProveedorNavigation.IdUsuarioNavigation.FirstName,

                MetodoPago = metodoPago.Nombre,
                MontoFormateado = nuevoPago.Monto.ToString("F2"),
                Moneda = nuevoPago.Moneda,
                CodigoReserva = nuevaReserva.CodigoReserva,
                DuracionPreReservaHoras = duracionPreReservaHoras,
                FechaExpiracionPreReserva = nuevaReserva.FechaExpiracionPreReserva,

                InformacionAdicional = $"Tu reserva ha sido creada con el código {nuevaReserva.CodigoReserva}. " +
                                      $"El encargado de la cancha se comunicará contigo para coordinar el pago. " +
                                      $"La pre-reserva expira el {nuevaReserva.FechaExpiracionPreReserva:dd/MM/yyyy HH:mm}."
            };

            response.UpdateData(reservaConPagoDto);
            response.AddOkResult($"Pre-reserva creada exitosamente. Código: {nuevaReserva.CodigoReserva}");

            try
            {
                var cliente = await _ReservaRepository.FindAll()
                    .Where(r => r.IdReserva == nuevaReserva.IdReserva)
                    .Select(r => r.IdUsuarioNavigation)
                    .FirstOrDefaultAsync();

                if(!operadores.Any())
                    operadores = new List<Operador>
                    {
                        new Operador
                        {
                            IdUsuarioNavigation = cancha.IdProveedorNavigation.IdUsuarioNavigation,
                        }
                    };

                if (cliente != null)
                {
                    await _notificacionService.NotificarNuevaReservaPendienteAsync(
                        nuevaReserva,
                        cancha,
                        cliente,
                        operadores.ToList()
                    );
                }
            }
            catch (Exception)
            {
                // No fallar la creación de la reserva si falla la notificación
                // El error ya fue logueado en el servicio de notificaciones
            }

            return response;
        }

        private async Task<string> GenerarCodigoReserva()
        {
            var codigo = await _ReservaRepository.ExecuteScalarSPAsync<string>("sp_GenerarCodigoReserva");
            if (!string.IsNullOrWhiteSpace(codigo))
                return codigo;

            var año = DateTime.Now.Year;

            var ultimoCodigoReserva = await _ReservaRepository.FindAll().
                Where(r => r.CodigoReserva != null && r.CodigoReserva.StartsWith($"RES-{año}-"))
                .OrderByDescending(r => r.CodigoReserva)
                .Select(r => r.CodigoReserva)
                .FirstOrDefaultAsync();

            int siguienteNumero = 1;

            if (!string.IsNullOrEmpty(ultimoCodigoReserva))
            {
                var partes = ultimoCodigoReserva.Split('-');
                if (partes.Length == 3 && int.TryParse(partes[2], out int numero))
                {
                    siguienteNumero = numero + 1;
                }
            }

            return $"RES-{año}-{siguienteNumero:D4}";
        }
    }
}