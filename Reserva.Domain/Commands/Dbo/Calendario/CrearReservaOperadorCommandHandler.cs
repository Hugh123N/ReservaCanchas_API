using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Calendario;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using Reserva.Repository.Security;

namespace Reserva.Domain.Commands.Dbo.Calendario
{
    public class CrearReservaOperadorCommandHandler : CommandHandlerBase<CrearReservaOperadorCommand, ReservaOperadorResponseDto>
    {
        private readonly IRepository<Entity.AspNetUsers> _userRepository;
        private readonly IRepository<Entity.Reserva> _reservaRepository;
        private readonly IRepository<Entity.DetalleReserva> _detalleReservaRepository;
        private readonly IRepository<Entity.Pago> _pagoRepository;
        private readonly IRepository<Entity.HorarioCancha> _horarioCanchaRepository;
        private readonly IRepository<Entity.Cancha> _canchaRepository;
        private readonly IRepository<Entity.Hora> _horaRepository;
        private readonly IRepository<Entity.EstadoReserva> _estadoReservaRepository;
        private readonly IRepository<Entity.EstadoPago> _estadoPagoRepository;
        private readonly IRepository<Entity.MetodoPago> _metodoPagoRepository;
        private readonly IRepository<Entity.Operador> _operadorRepository;
        private readonly IUserIdentity _userIdentity;
        private readonly UserManager<Entity.ApplicationUser> _UserManager;
        private readonly IRepository<Entity.ApplicationUser> _applicationUserRepository;

        public CrearReservaOperadorCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CrearReservaOperadorCommandValidator validator,
            IRepository<Entity.AspNetUsers> userRepository,
            IRepository<Entity.Reserva> reservaRepository,
            IRepository<Entity.DetalleReserva> detalleReservaRepository,
            IRepository<Entity.Pago> pagoRepository,
            IRepository<Entity.HorarioCancha> horarioCanchaRepository,
            IRepository<Entity.Cancha> canchaRepository,
            IRepository<Entity.Hora> horaRepository,
            IRepository<Entity.EstadoReserva> estadoReservaRepository,
            IRepository<Entity.EstadoPago> estadoPagoRepository,
            IRepository<Entity.MetodoPago> metodoPagoRepository,
            UserManager<Entity.ApplicationUser> UserManager,
            IRepository<Entity.ApplicationUser> applicationUserRepository,
        IUserIdentity userIdentity,
        IRepository<Entity.Operador> operadorRepository) : base(unitOfWork, mapper, mediator, validator)
        {
            _userRepository = userRepository;
            _reservaRepository = reservaRepository;
            _detalleReservaRepository = detalleReservaRepository;
            _pagoRepository = pagoRepository;
            _horarioCanchaRepository = horarioCanchaRepository;
            _canchaRepository = canchaRepository;
            _horaRepository = horaRepository;
            _estadoReservaRepository = estadoReservaRepository;
            _estadoPagoRepository = estadoPagoRepository;
            _metodoPagoRepository = metodoPagoRepository;
            _operadorRepository = operadorRepository;
            _userIdentity = userIdentity;
            _UserManager = UserManager;
            _applicationUserRepository = applicationUserRepository;
        }

        public override async Task<ResponseDto<ReservaOperadorResponseDto>> HandleCommand(CrearReservaOperadorCommand request,CancellationToken cancellationToken)
        {
            var response = new ResponseDto<ReservaOperadorResponseDto>();
            var idUserCurrent = _userIdentity.GetCurrentUserId();
            if (idUserCurrent == null)
            {
                response.AddErrorResult("No se pudo obtener el usuario actual");
                return response;
            }   

            try
            {
                var dto = request.RequestDto;

                var cliente = await ObtenerOCrearCliente(dto.Cliente);
                if (cliente == null)
                {
                    response.AddErrorResult("No se pudo obtener o crear el cliente");
                    return response;
                }

                var disponible = await ValidarDisponibilidadHorarios(dto.IdCancha, dto.Horarios);
                if (!disponible.Item1)
                {
                    response.AddErrorResult(disponible.Item2);
                    return response;
                }

                var cancha = await _canchaRepository.GetByAsync(x => x.IdCancha == dto.IdCancha && x.Activo);
                
                var codigoReserva = await GenerarCodigoReserva();

                string codigoEstadoReserva = dto.TipoReserva == TipoReservaOperador.Inmediata
                    ? Constants.ESTADO_RESERVA.Confirmado
                    : Constants.ESTADO_RESERVA.Pendiente;

                var estadoReserva = await _estadoReservaRepository.GetByAsync(
                    x => x.Codigo == codigoEstadoReserva && x.Activo);

                if (estadoReserva == null)
                {
                    response.AddErrorResult($"Estado de reserva '{codigoEstadoReserva}' no encontrado");
                    return response;
                }

                // Buscar operador si es necesario
                int? idOperador = null;
                if (dto.TipoReserva == TipoReservaOperador.Inmediata && idUserCurrent != Guid.Empty)
                {
                    var operador = await _operadorRepository.GetByAsync(
                        x => x.IdUsuario == idUserCurrent && x.Activo);
                    idOperador = operador?.IdOperador;
                }

                // 9. Obtener primera fecha de los horarios
                var primeraFecha = dto.Horarios.Min(h => h.Fecha);

                // 10. Crear registro de Reserva
                var pago = await CrearRegistroPago(dto.Pago.MontoTotal, dto.Pago, dto.TipoReserva, idUserCurrent ?? new Guid());

                var reserva = new Entity.Reserva
                {
                    CodigoReserva = codigoReserva,
                    IdCliente = cliente.Id,
                    IdCancha = dto.IdCancha,
                    IdTipoDeporte = dto.IdTipoDeporte,
                    FechaReserva = primeraFecha,
                    MontoTotal = dto.Pago.MontoTotal,
                    IdEstadoReserva = estadoReserva.IdEstadoReserva,
                    Observaciones = dto.Observaciones,
                    Pago = new List<Entity.Pago> { pago },
                };

                // Si es reserva inmediata, agregar info de confirmación
                if (dto.TipoReserva == TipoReservaOperador.Inmediata)
                {
                    reserva.IdOperadorConfirmo = idOperador;
                    reserva.FechaConfirmacion = DateTimeOffset.UtcNow;
                    // TODO: ENVIAR NOTIFICAICON AL CLIENTE
                }

                await _reservaRepository.AddAsync(reserva);
                await _reservaRepository.SaveAsync();

                await CrearDetallesReserva(reserva.IdReserva, dto.IdCancha, dto.Horarios);

                // 11. Construir respuesta
                var responseData = new ReservaOperadorResponseDto
                {
                    IdReserva = reserva.IdReserva,
                    CodigoReserva = reserva.CodigoReserva,
                    FechaReserva = reserva.FechaReserva,
                    MontoTotal = reserva.MontoTotal,
                    EstadoReserva = codigoEstadoReserva,
                    IdPago = pago.IdPago,
                    EstadoPago = pago.IdEstadoPagoNavigation?.Codigo ?? "",
                    NombreCliente = $"{cliente.FirstName} {cliente.LastName}",
                    TelefonoCliente = cliente.PhoneNumber,
                    NombreCancha = cancha.Nombre,
                    Observaciones = reserva.Observaciones
                };

                response.UpdateData(responseData);
                response.AddOkResult("Reserva creada exitosamente");
            }
            catch (Exception ex)
            {
                response.AddErrorResult($"Error al crear reserva: {ex}");
            }

            return response;
        }

        private async Task<Entity.AspNetUsers?> ObtenerOCrearCliente(ClienteReservaDto clienteDto)
        {
            if (clienteDto.IdCliente.HasValue)
            {
                return await _userRepository.GetByAsync(
                    x => x.Id == clienteDto.IdCliente.Value && x.Activo);
            }

            if (clienteDto.EsNuevoCliente)
            {
                var nombres = clienteDto.NombreCompleto?.Split(' ', 2) ?? new[] { "", "" };
                var firstName = nombres[0];
                var lastName = nombres.Length > 1 ? nombres[1] : "";

                var nuevoCliente = new Entity.ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = clienteDto.Telefono,
                    Email = clienteDto.Email,
                    UserName = clienteDto.Email,
                    EmailConfirmed = false,
                };

                _applicationUserRepository.UpdateAuditTrails(nuevoCliente);
                await _UserManager.CreateAsync(nuevoCliente);

                var cliente = _mapper!.Map<Entity.AspNetUsers>(nuevoCliente);

                return cliente;
            }

            return null;
        }

        private async Task<(bool, string)> ValidarDisponibilidadHorarios(int idCancha,List<BloqueHorarioDto> horarios)
        {
            foreach (var bloque in horarios)
            {
                var diaSemana = (int)bloque.Fecha.DayOfWeek;
                if (diaSemana == 0) diaSemana = 7;

                // Obtener todos los HorarioCancha en el rango
                // IdHorarioCanchaFin es INCLUSIVO (representa el último slot seleccionado)
                var horariosEnRango = await _horarioCanchaRepository.FindByAsync(
                    hc => hc.IdHorarioCancha >= bloque.IdHorarioCanchaInicio
                       && hc.IdHorarioCancha <= bloque.IdHorarioCanchaFin
                       && hc.IdCancha == idCancha
                       && hc.IdDiaSemana == diaSemana
                       && hc.Activo,
                    hc => hc.IdHoraInicioNavigation,
                    hc => hc.IdHoraFinNavigation);

                if (!horariosEnRango.Any())
                {
                    return (false, $"No se encontraron horarios configurados para el rango seleccionado");
                }

                // Verificar disponibilidad de cada horario
                foreach (var horarioCancha in horariosEnRango.OrderBy(h => h.IdHoraInicio))
                {
                    var reservasExistentes = await _detalleReservaRepository.FindByAsync(dr =>
                        dr.IdHorarioCancha == horarioCancha.IdHorarioCancha
                        && dr.Activo
                        && dr.IdReservaNavigation != null
                        && dr.IdReservaNavigation.Activo
                        && dr.IdReservaNavigation.FechaReserva.Date == bloque.Fecha.Date
                        && (dr.IdReservaNavigation.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Confirmado
                            || dr.IdReservaNavigation.IdEstadoReservaNavigation.Codigo == Constants.ESTADO_RESERVA.Pendiente),
                        dr => dr.IdReservaNavigation!,
                        dr => dr.IdReservaNavigation!.IdEstadoReservaNavigation);

                    if (reservasExistentes.Any())
                    {
                        var hora = horarioCancha.IdHoraInicioNavigation?.HoraTexto ?? horarioCancha.IdHoraInicio.ToString();
                        return (false, $"El horario del {bloque.Fecha:dd/MM/yyyy} a las {hora} ya está reservado");
                    }
                }
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// Crea los registros de DetalleReserva (uno por cada hora)
        /// </summary>
        private async Task CrearDetallesReserva(int idReserva, int idCancha, List<BloqueHorarioDto> horarios)
        {
            foreach (var bloque in horarios)
            {
                var diaSemana = (int)bloque.Fecha.DayOfWeek;
                if (diaSemana == 0) diaSemana = 7;

                // Obtener todos los HorarioCancha en el rango
                // IdHorarioCanchaFin es INCLUSIVO (representa el último slot seleccionado)
                var horariosEnRango = await _horarioCanchaRepository.FindByAsync(
                    hc => hc.IdHorarioCancha >= bloque.IdHorarioCanchaInicio
                       && hc.IdHorarioCancha <= bloque.IdHorarioCanchaFin
                       && hc.IdCancha == idCancha
                       && hc.IdDiaSemana == diaSemana
                       && hc.Activo);

                // Crear un detalle por cada HorarioCancha en el rango
                foreach (var horarioCancha in horariosEnRango.OrderBy(h => h.IdHoraInicio))
                {
                    var detalle = new Entity.DetalleReserva
                    {
                        IdReserva = idReserva,
                        IdHorarioCancha = horarioCancha.IdHorarioCancha,
                        Activo = true
                    };

                    await _detalleReservaRepository.AddAsync(detalle);
                }
            }

            await _detalleReservaRepository.SaveAsync();
        }

        private async Task<Entity.Pago> CrearRegistroPago(decimal montoTotal,PagoReservaOperadorDto pagoDto,
            TipoReservaOperador tipoReserva,
            Guid idUsuarioOperador)
        {
            decimal montoPagado = tipoReserva == TipoReservaOperador.Inmediata
                ? (pagoDto.MontoPagado ?? 0)
                : 0;

            decimal montoPendiente = montoTotal - montoPagado;

            string codigoEstadoPago;
            if (montoPagado == 0)
            {
                codigoEstadoPago = Constants.ESTADO_PAGO.Pendiente;
            }
            else if (montoPagado >= montoTotal)
            {
                codigoEstadoPago = Constants.ESTADO_PAGO.Pagado;
            }
            else
            {
                codigoEstadoPago = Constants.ESTADO_PAGO.Parcial;
            }

            var estadoPago = await _estadoPagoRepository.GetByAsync(x => x.Codigo == codigoEstadoPago && x.Activo);
            var metodoPago = await _metodoPagoRepository.GetByAsync(x => x.Codigo == pagoDto.CodigoMetodoPago && x.Activo);

            int? idOperador = null;
            if (idUsuarioOperador != Guid.Empty)
            {
                var operador = await _operadorRepository.GetByAsync(x => x.IdUsuario == idUsuarioOperador && x.Activo);
                idOperador = operador?.IdOperador;
            }

            var pago = new Entity.Pago
            {
                Monto = montoTotal,
                MontoAdelanto = montoPagado,
                MontoPendiente = montoPendiente,
                IdEstadoPago = estadoPago?.IdEstadoPago ?? 0,
                IdMetodoPago = metodoPago?.IdMetodoPago ?? 0,
                CodigoOperacion = pagoDto.CodigoOperacion,
                NumeroReferencia = pagoDto.NumeroReferencia,
                IdOperador = idOperador,
                Moneda = "PEN", // Default to PEN (Soles)
                IdEstadoPagoNavigation = estadoPago!
            };

            return pago;
        }

        private async Task<string> GenerarCodigoReserva()
        {
            var codigo = await _reservaRepository.ExecuteScalarSPAsync<string>("sp_GenerarCodigoReserva");

            if (!string.IsNullOrWhiteSpace(codigo))
                return codigo;

            var año = DateTime.Now.Year;

            var ultimoCodigoReserva = await _reservaRepository.FindAll().
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
