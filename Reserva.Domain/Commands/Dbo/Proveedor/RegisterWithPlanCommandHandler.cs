using AutoMapper;
using MediatR;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Commands.Dbo.ProveedorPlan;
using Reserva.Domain.Commands.Dbo.Usuario;
using Reserva.Domain.Commands.User;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Proveedor;
using Reserva.Dto.Dbo.ProveedorPlan;
using Reserva.Dto.Dbo.Usuario;
using Reserva.Dto.User;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using Reserva.Repository.Utils;

namespace Reserva.Domain.Commands.Dbo.Proveedor
{
    /// <summary>
    /// Handler que orquesta el registro completo de un proveedor con plan gratuito:
    /// 1. Crear usuario (CreateUsuarioCommand)
    /// 2. Crear proveedor (CreateProveedorCommand)
    /// 3. Crear proveedor plan (CreateProveedorPlanCommand)
    /// 4. Login (LoginCommand)
    /// </summary>
    public class RegisterWithPlanCommandHandler : CommandHandlerBase<RegisterWithPlanCommand, LoginResultDto>
    {
        private readonly IRepository<Entity.Proveedor> _proveedorRepository;
        private readonly IRepository<Entity.EstadoProveedor> _estadoProveedorRepository;

        public RegisterWithPlanCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            RegisterWithPlanCommandValidator validator,
            IRepository<Entity.Proveedor> proveedorRepository,
            IRepository<Entity.EstadoProveedor> estadoProveedorRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _proveedorRepository = proveedorRepository;
            _estadoProveedorRepository = estadoProveedorRepository;
        }

        public override async Task<ResponseDto<LoginResultDto>> HandleCommand(RegisterWithPlanCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<LoginResultDto>();
            var dto = request.Dto;

            // 1. Crear proveedor
            var createUsuarioDto = new CreateProveedorDto
            {
                UserName = dto.UserName,
                Email = dto.Email,
                Password = dto.Password,
                ConfirmPassword = dto.ConfirmPassword,
                Telefono = dto.Telefono,
                Nombre = dto.Nombre,
                Apellidos = dto.Apellidos
            };

            var proveedorResponse = await _mediator.Send(new CreateProveedorCommand(createUsuarioDto), cancellationToken);

            if (proveedorResponse.Data == null || !proveedorResponse.IsValid)
            {
                response.Messages = proveedorResponse.Messages;
                return response;
            }

            // 2. Crear proveedor plan (plan gratuito)
            var now = DateTimeOffset.UtcNow;
            var createProveedorPlanDto = new CreateProveedorPlanDto
            {
                IdProveedor = proveedorResponse.Data.IdProveedor,
                IdPlane = dto.IdPlane,
                IdPlanTarifa = dto.IdPlanTarifa,
                FechaInicio = now,
                FechaFin = DateTimeHelper.GetNextBillingDate(now, now.Day, 30), // 30 días de prueba
                Estado = Constants.ESTADO_PROV_PLAN.ACTIVE,
                AutoRenovacion = false,
                EsActual = true,
                EsPruebaGratis = true
            };

            var planResponse = await _mediator.Send(new CreateProveedorPlanCommand(createProveedorPlanDto), cancellationToken);

            if (!planResponse.IsValid)
            {
                response.Messages = planResponse.Messages;
                return response;
            }

            // 4. Login - generar token
            var loginDto = new LoginDto
            {
                ApplicationCode = "",
                UserName = dto.UserName,
                Password = dto.Password,
                RememberMe = false
            };

            var loginResponse = await _mediator.Send(new LoginCommand(loginDto), cancellationToken);

            if (loginResponse.Data == null || !loginResponse.IsValid)
            {
                response.Messages = loginResponse.Messages;
                return response;
            }

            // Retornar token de acceso
            response.UpdateData(loginResponse.Data);
            response.AddOkResult("Registro exitoso. Bienvenido a ReservaCanchas.");

            return await Task.FromResult(response);
        }
    }
}
