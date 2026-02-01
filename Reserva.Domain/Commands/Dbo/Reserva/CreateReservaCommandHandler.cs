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
        private readonly IRepository<Entity.DetalleReserva> _DetalleReservaRepository;
        private readonly IRepository<Entity.HorarioCancha> _HorarioCanchaRepository;
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
            IRepository<Entity.DetalleReserva> DetalleReservaRepository,
            IRepository<Entity.HorarioCancha> HorarioCanchaRepository,
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
            _DetalleReservaRepository = DetalleReservaRepository;
            _HorarioCanchaRepository = HorarioCanchaRepository;
            _configuration = configuration;
            _notificacionService = notificacionService;
        }

        public override async Task<ResponseDto<ReservaConPagoDto>> HandleCommand(CreateReservaCommand request,CancellationToken cancellationToken)
        {
            var response = new ResponseDto<ReservaConPagoDto>();

            var cancha = await _CanchaRepository.GetByAsync(
                c => c.IdCancha == request.CreateDto.IdCancha,
                c => c.IdProveedorNavigation!,
                c => c.IdProveedorNavigation!.ConfiguracionProveedor!,
                c => c.IdProveedorNavigation!.IdUsuarioNavigation,
                c => c.OperadorCancha);

            var metodoPago = await _MetodoPagoRepository.GetByAsync(mp => mp.Codigo == request.CreateDto.CodigoMetodoPago);

            var reservasDelDia = await _ReservaRepository.FindByAsNoTrackingAsync(
                r => r.IdCancha == request.CreateDto.IdCancha
                     && r.FechaReserva.Date == request.CreateDto.FechaReserva.Date
                     && r.Activo
                     && r.IdEstadoReservaNavigation.Codigo != Constants.ESTADO_RESERVA.Cancelado
                     && r.IdEstadoReservaNavigation.Codigo != Constants.ESTADO_RESERVA.Expirado,
                r => r.DetalleReserva
            );

            // Obtener los IDs de horarios ya reservados
            var horariosReservados = reservasDelDia
                .SelectMany(r => r.DetalleReserva)
                .Where(d => d.IdHorarioCancha.HasValue && d.Activo)
                .Select(d => d.IdHorarioCancha.Value)
                .ToHashSet();

            // Validar si algún horario ya está reservado
            var horariosConflicto = request.CreateDto.IdsHorarioCancha
                .Where(id => horariosReservados.Contains(id))
                .ToList();

            if (horariosConflicto.Any())
            {
                response.AddErrorResult($"Los siguientes horarios ya están reservados: {string.Join(", ", horariosConflicto)}");
                return response;
            }

            var estadoPendienteReserva = await _EstadoReservaRepository.GetByAsNoTrackingAsync(
                x => x.Codigo!.Equals(Constants.ESTADO_RESERVA.Pendiente));

            var nuevaReserva = _mapper?.Map<Entity.Reserva>(request.CreateDto);

            nuevaReserva!.DetalleReserva = request.CreateDto.IdsHorarioCancha.Select(idHorario => new Entity.DetalleReserva
            {
                IdHorarioCancha = idHorario
            }).ToList();

            // Obtener el primer horario para combinar la fecha con su hora de inicio
            if (request.CreateDto.IdsHorarioCancha != null && request.CreateDto.IdsHorarioCancha.Any())
            {
                var primerIdHorario = request.CreateDto.IdsHorarioCancha.OrderBy(id => id).First();

                var primerHorarioCancha = await _HorarioCanchaRepository.GetByAsync(
                    hc => hc.IdHorarioCancha == primerIdHorario && hc.Activo,
                    hc => hc.IdHoraInicioNavigation);

                if (primerHorarioCancha != null && primerHorarioCancha.IdHoraInicioNavigation != null)
                {
                    var horaInicio = primerHorarioCancha.IdHoraInicioNavigation.Hora1;
                    var fechaOriginal = request.CreateDto.FechaReserva;

                    // Combinar la fecha con la hora del primer horario
                    nuevaReserva.FechaReserva = new DateTimeOffset(fechaOriginal.Year, fechaOriginal.Month, fechaOriginal.Day,
                        horaInicio.Hour, horaInicio.Minute, horaInicio.Second, fechaOriginal.Offset);
                }
            }

            int duracionPreReservaHoras = cancha.IdProveedorNavigation.ConfiguracionProveedor!.DuracionPreReserva ?? 12;
            nuevaReserva.FechaExpiracionPreReserva = DateTimeOffset.Now.AddHours(duracionPreReservaHoras);
            nuevaReserva.CodigoReserva = await GenerarCodigoReserva();
            nuevaReserva.IdEstadoReserva = estadoPendienteReserva!.IdEstadoReserva;
            //nuevaReserva.IdCanchaNavigation = null;
            nuevaReserva.RecordatorioEnviado = false;
            nuevaReserva.NotificacionAdvertenciaEnviada = false;

            await _ReservaRepository.AddAsync(nuevaReserva);
            await _ReservaRepository.SaveAsync();

            decimal montoTotal = request.CreateDto.MontoTotal;

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
                    .Select(r => r.IdClienteNavigation)
                    .FirstOrDefaultAsync();

                if(!operadores.Any())
                    operadores = new List<Entity.Operador>
                    {
                        new Entity.Operador
                        {
                            IdUsuarioNavigation = cancha.IdProveedorNavigation.IdUsuarioNavigation,
                        }
                    };

                // Cargar DetalleReserva con sus navegaciones para formatear horarios
                var detallesConHorarios = await _DetalleReservaRepository.FindByAsNoTrackingAsync(
                    d => d.IdReserva == nuevaReserva.IdReserva && d.Activo,
                    d => d.IdHorarioCanchaNavigation!.IdHoraInicioNavigation!,
                    d => d.IdHorarioCanchaNavigation!.IdHoraFinNavigation!
                );

                // Extraer horarios y formatear
                var horariosLista = detallesConHorarios
                    .Where(d => d.IdHorarioCanchaNavigation?.IdHoraInicioNavigation != null
                             && d.IdHorarioCanchaNavigation?.IdHoraFinNavigation != null)
                    .Select(d => {
                        var horaInicio = d.IdHorarioCanchaNavigation!.IdHoraInicioNavigation!.Hora1;
                        var horaFin = d.IdHorarioCanchaNavigation!.IdHoraFinNavigation!.Hora1;
                        return (inicio: horaInicio, fin: horaFin);
                    })
                    .OrderBy(h => h.inicio)
                    .ToList();

                var horariosFormateado = NotificacionService.FormatearHorariosConsecutivos(horariosLista);

                if (cliente != null)
                {
                    await _notificacionService.NotificarNuevaReservaPendienteAsync(
                        nuevaReserva,
                        cancha,
                        cliente,
                        operadores.ToList(),
                        horariosFormateado
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