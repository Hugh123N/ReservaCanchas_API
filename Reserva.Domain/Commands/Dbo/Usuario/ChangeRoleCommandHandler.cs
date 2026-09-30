using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Commands.Dbo.ProveedorPlan;
using Reserva.Domain.Commands.Token;
using Reserva.Domain.Commands.User;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.ProveedorPlan;
using Reserva.Dto.Dbo.Usuario;
using Reserva.Dto.User;
using Reserva.Entity;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using Reserva.Repository.Data;
using Reserva.Repository.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Domain.Commands.Dbo.Usuario
{
    public class ChangeRoleCommandHandler : CommandHandlerBase<ChangeRoleCommand, LoginResultDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<ApplicationRole> _rolRepository;
        private readonly IRepository<Entity.Proveedor> _proveedorRepository;
        private readonly IRepository<Entity.Operador> _operadorRepository;
        private readonly IRepository<Entity.EstadoProveedor> _estadoProveedorRepository;
        private readonly ReservaCanchasContext _dbContext;

        public ChangeRoleCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            UserManager<ApplicationUser> userManager,
            IRepository<ApplicationRole> rolRepository,
            IRepository<Entity.Proveedor> proveedorRepository,
            IRepository<Entity.Operador> operadorRepository,
            IRepository<Entity.EstadoProveedor> estadoProveedorRepository,
            ReservaCanchasContext dbContext
        ) : base(unitOfWork, mapper, mediator)
        {
            _userManager = userManager;
            _rolRepository = rolRepository;
            _proveedorRepository = proveedorRepository;
            _operadorRepository = operadorRepository;
            _estadoProveedorRepository = estadoProveedorRepository;
            _dbContext = dbContext;
        }

        public override async Task<ResponseDto<LoginResultDto>> HandleCommand(ChangeRoleCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<LoginResultDto>();
            var targetRole = request.ChangeRoleDto.TargetRole.ToUpper();
            var normalizedTargetRole = targetRole;

            var applicationUser = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (applicationUser == null)
            {
                response.AddErrorResult("Usuario no encontrado.");
                return response;
            }

            var currentRoles = await _userManager.GetRolesAsync(applicationUser);
            if (currentRoles.Any(r => r.Equals(targetRole, StringComparison.OrdinalIgnoreCase)))
            {
                response.AddErrorResult($"El usuario ya tiene el rol {targetRole}.");
                return response;
            }

            var rolesToAdd = new List<string> { normalizedTargetRole };
            
            // Si se hace Proveedor, también agregar Cliente y Operador (como en el flujo original)
            if (targetRole.Equals(Constants.Role.Proveedor, StringComparison.OrdinalIgnoreCase))
            {
                rolesToAdd.Add(Constants.Role.Cliente.ToUpper());
                rolesToAdd.Add(Constants.Role.Operador.ToUpper());
            }

            // Agregar roles (el UnitOfWork maneja transacción automáticamente)
            var addRoleResult = await _userManager.AddToRolesAsync(applicationUser, rolesToAdd);
            if (!addRoleResult.Succeeded)
            {
                addRoleResult.Errors.ToList().ForEach(e => response.AddErrorResult($"Error al asignar rol: {e.Code}: {e.Description}"));
                return response;
            }

            // Crear entidad según el rol
            if (targetRole.Equals(Constants.Role.Proveedor, StringComparison.OrdinalIgnoreCase))
            {
                var proveedor = _mapper?.Map<Entity.Proveedor>(request.ChangeRoleDto);
                if (proveedor == null)
                {
                    response.AddErrorResult("No se pudo mapear la información del proveedor.");
                    await _userManager.RemoveFromRolesAsync(applicationUser, rolesToAdd);
                    return response;
                }

                proveedor.IdUsuario = applicationUser.Id;
                proveedor.IdEstadoProveedor = 1; // Pendiente

                await _proveedorRepository.AddAsync(proveedor);
                // NO llamar SaveAsync() - UnitOfWork hace commit automático

                // Crear ProveedorPlan (plan gratuito de prueba por 30 días)
                var now = DateTimeOffset.UtcNow;
                var createProveedorPlanDto = new CreateProveedorPlanDto
                {
                    IdProveedor = proveedor.IdProveedor,
                    IdPlane = request.ChangeRoleDto.IdPlane,
                    IdPlanTarifa = request.ChangeRoleDto.IdPlanTarifa,
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
                    await _userManager.RemoveFromRolesAsync(applicationUser, rolesToAdd);
                    return response;
                }
            }
            else if (targetRole.Equals(Constants.Role.Operador, StringComparison.OrdinalIgnoreCase))
            {
                var operador = new Entity.Operador
                {
                    Nombres = applicationUser.FirstName ?? "",
                    Apellidos = applicationUser.LastName ?? "",
                    Telefono = applicationUser.PhoneNumber,
                    Email = applicationUser.Email,
                    IdUsuario = applicationUser.Id
                };

                await _operadorRepository.AddAsync(operador);
                // NO llamar SaveAsync() - UnitOfWork hace commit automático
            }

            // Login - generar token con el nuevo rol
            var roles = await _userManager.GetRolesAsync(applicationUser);
            int? idUsuarioNegocio = null;

            if (roles.Any(r => r.Equals(Constants.Role.Proveedor, StringComparison.OrdinalIgnoreCase)))
            {
                var proveedor = await _proveedorRepository.GetByAsNoTrackingAsync(p => p.IdUsuario == applicationUser.Id && p.Activo);
                if (proveedor != null)
                {
                    idUsuarioNegocio = proveedor.IdProveedor;
                }
            }
            else if (roles.Any(r => r.Equals(Constants.Role.Operador, StringComparison.OrdinalIgnoreCase)))
            {
                var operador = await _operadorRepository.GetByAsNoTrackingAsync(o => o.IdUsuario == applicationUser.Id && o.Activo);
                if (operador != null)
                {
                    idUsuarioNegocio = operador.IdOperador;
                }
            }

            var accessToken = await _mediator.Send(new GenerateTokenCommand("", applicationUser, idUsuarioNegocio), cancellationToken)!;

            if (accessToken?.Data == null)
            {
                response.AddErrorResult("Error al generar token.");
                return response;
            }

            response.UpdateData(new LoginResultDto { AccessToken = accessToken.Data });
            response.AddOkResult("Rol actualizado exitosamente.");

            return response;
        }
    }
}