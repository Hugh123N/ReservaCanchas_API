using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Commands.Token;
using Reserva.Dto.Base;
using Reserva.Dto.User;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using System.Runtime.Intrinsics.X86;

namespace Reserva.Domain.Commands.User
{
    public class LoginCommandHandler : CommandHandlerBase<LoginCommand, LoginResultDto>
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<Entity.ApplicationUser> _userManager;
        private readonly SignInManager<Entity.ApplicationUser> _signInManager;
        private readonly IRepository<Entity.Operador> _OperadorRepository;
        private readonly IRepository<Entity.Proveedor> _ProveedorRepository;

        public LoginCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            LoginCommandValidator validator,
            IConfiguration configuration,
            UserManager<Entity.ApplicationUser> userManager,
            SignInManager<Entity.ApplicationUser> signInManager,
            IRepository<Entity.Operador> OperadorRepository,
            IRepository<Entity.Proveedor> ProveedorRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
            _OperadorRepository = OperadorRepository;
            _ProveedorRepository = ProveedorRepository;
        }


        public override async Task<ResponseDto<LoginResultDto>> HandleCommand(LoginCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<LoginResultDto>();
            var lockoutOnFailure = _configuration.GetValue<bool>("SignInOptions:LockoutEnabled");
            var login = request.LoginDto;

            var user = await _userManager.FindByNameAsync(login.UserName)
               ?? await _userManager.FindByEmailAsync(login.UserName);

            var result = await _signInManager.PasswordSignInAsync(user.UserName, request.LoginDto.Password, request.LoginDto.RememberMe, lockoutOnFailure: lockoutOnFailure);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                    response.AddErrorResult("Usuario bloqueado temporalmente por múltiples intentos fallidos.");
                else if (result.IsNotAllowed)
                    response.AddErrorResult("El usuario no está permitido para iniciar sesión.");
                else
                    response.AddErrorResult("No se pudo iniciar sesión.");
                return response;
            }

            int? idUsuarioNegocio = null;
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Any(r => r.Equals(Constants.Role.Proveedor, StringComparison.OrdinalIgnoreCase)))
            {
                var proveedor = await _ProveedorRepository.GetByAsNoTrackingAsync(
                    p => p.IdUsuario == user.Id && p.Activo
                );

                if (proveedor != null)
                {
                    idUsuarioNegocio = proveedor.IdProveedor;
                }
            }
            else if (roles.Any(r => r.Equals(Constants.Role.Operador, StringComparison.OrdinalIgnoreCase)))
            {
                var operador = await _OperadorRepository.GetByAsNoTrackingAsync(
                    o => o.IdUsuario == user.Id && o.Activo
                );

                if (operador != null)
                {
                    idUsuarioNegocio = operador.IdOperador;
                }
            }

            var accessToken = await _mediator.Send(new GenerateTokenCommand(request.LoginDto.ApplicationCode, user, idUsuarioNegocio), cancellationToken)!;

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
