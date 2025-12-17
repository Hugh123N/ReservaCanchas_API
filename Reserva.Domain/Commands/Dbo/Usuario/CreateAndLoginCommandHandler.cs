using AutoMapper;
using Google.Apis.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Commands.LoginExternalProvider;
using Reserva.Domain.Commands.Token;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Usuario;
using Reserva.Dto.User;
using Reserva.Entity;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Domain.Commands.Dbo.Usuario
{
    internal class CreateAndLoginCommandHandler : CommandHandlerBase<CreateAndLoginCommand, LoginResultDto>
    {
        private readonly IRepository<Entity.AspNetUsers> _UsuarioRepository;
        private readonly UserManager<Entity.ApplicationUser> _UserManager;
        private readonly SignInManager<Entity.ApplicationUser> _SignInManager;
        private readonly IRepository<AspNetRoles> _RolRepository;
        private readonly IConfiguration _configuration;
        private readonly IRepository<Entity.ApplicationUser> _applicationUserRepository;
        private readonly IRepository<Entity.EstadoUsuario> _EstadoUsuarioRepository;
        private readonly IRepository<Entity.Operador> _OperadorRepository;
        private readonly IRepository<Entity.Proveedor> _ProveedorRepository;

        public CreateAndLoginCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            IRepository<Entity.AspNetUsers> UsuarioRepository,
            UserManager<Entity.ApplicationUser> userManager,
            IRepository<Entity.ApplicationUser> ApplicationUserRepository,
            IRepository<Entity.AspNetRoles> RolRepository,
            IRepository<Entity.EstadoUsuario> EstadoUsuarioRepository,
            SignInManager<Entity.ApplicationUser> SignInManager,
            IConfiguration configuration,
            IRepository<Entity.Operador> OperadorRepository,
            IRepository<Entity.Proveedor> ProveedorRepository
        ) : base(unitOfWork, mapper, mediator)
        {
            _UsuarioRepository = UsuarioRepository;
            _UserManager = userManager;
            _configuration = configuration;
            _RolRepository = RolRepository;
            _applicationUserRepository = ApplicationUserRepository;
            _EstadoUsuarioRepository = EstadoUsuarioRepository;
            _SignInManager = SignInManager;
            _OperadorRepository = OperadorRepository;
            _ProveedorRepository = ProveedorRepository;
        }


        public override async Task<ResponseDto<LoginResultDto>> HandleCommand(CreateAndLoginCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<LoginResultDto>();
            var nuevoUsuario = new Entity.ApplicationUser();
            var estadoUsuario = await _EstadoUsuarioRepository.GetByAsNoTrackingAsync(x => x.Codigo.Equals(Constants.ESTADO_USUARIO.Activo));

            var lockoutOnFailure = _configuration.GetValue<bool>("SignInOptions:LockoutEnabled");

            if (!request.CreateDto.TypeValidation.Contains(Constants.TIPO_VALIDACION.CORREO))
            {
                var responseUser = await _mediator!.Send(new ExternalProviderCommand(request.CreateDto));

                if (responseUser?.Data == null)
                {
                    response.AddErrorResult("Error al validar el token con el proveedor externo.");
                    return response;
                }
                nuevoUsuario = responseUser.Data;

                var usuarioExistente = await _UserManager.FindByEmailAsync(nuevoUsuario.Email);
                if (usuarioExistente == null)
                {
                    _applicationUserRepository.UpdateAuditTrails(nuevoUsuario);
                    await _UserManager.CreateAsync(nuevoUsuario);
                }
                else
                {
                    nuevoUsuario = usuarioExistente;
                }

                if (request.CreateDto.TipoUser != null && request.CreateDto.TipoUser.Equals("Proveedor")) {
                    var normalizedRoleNames = new List<string>
                    {
                        Constants.Role.Proveedor.ToUpper(),
                        Constants.Role.Operador.ToUpper()
                    };

                    var addRoleResult = await _UserManager.AddToRolesAsync(nuevoUsuario, normalizedRoleNames);
                }
            }
            else
            {
                var result = await _SignInManager.PasswordSignInAsync(request.CreateDto.Email, request.CreateDto.Password, true, lockoutOnFailure: lockoutOnFailure);

                if (!result.Succeeded)
                {
                    response.AddErrorResult(Resources.User.LoginAccessTokenError);
                    return response;
                }

                nuevoUsuario = await _UserManager.FindByEmailAsync(request.CreateDto.Email);

                if (nuevoUsuario == null)
                {
                    response.AddErrorResult("Usuario no encontrado después de login.");
                    return response;
                }
            }

            int? idUsuarioNegocio = null;
            var roles = await _UserManager.GetRolesAsync(nuevoUsuario);

            if (roles.Contains(Constants.Role.Proveedor))
            {
                var proveedor = await _ProveedorRepository.GetByAsNoTrackingAsync(
                    p => p.IdUsuario == nuevoUsuario.Id && p.Activo
                );

                if (proveedor != null)
                {
                    idUsuarioNegocio = proveedor.IdProveedor;
                }
            }
            else if (roles.Contains(Constants.Role.Operador))
            {
                var operador = await _OperadorRepository.GetByAsNoTrackingAsync(
                    o => o.IdUsuario == nuevoUsuario.Id && o.Activo
                );

                if (operador != null)
                {
                    idUsuarioNegocio = operador.IdOperador;
                }
            }

            var accessToken = await _mediator.Send(new GenerateTokenCommand(request.CreateDto.ApplicationCode, nuevoUsuario, idUsuarioNegocio), cancellationToken)!;

            if (accessToken?.Data == null)
            {
                response.AddErrorResult("Error al generar token.");
                return response;
            }

            response.UpdateData(new LoginResultDto { AccessToken = accessToken.Data });
            response.AddOkResult("Login exitoso.");
            return response;
        }
    }
}
