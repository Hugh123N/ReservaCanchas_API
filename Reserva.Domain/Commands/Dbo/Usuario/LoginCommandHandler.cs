using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Commands.Token;
using Reserva.Domain.Queries.Dbo.Usuario;
using Reserva.Dto.Base;
using Reserva.Dto.User;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.User
{
    public class LoginCommandHandler : CommandHandlerBase<LoginCommand, LoginResultDto>
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<Entity.ApplicationUser> _userManager;
        private readonly SignInManager<Entity.ApplicationUser> _signInManager;

        public LoginCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            LoginCommandValidator validator,
            IConfiguration configuration,
            UserManager<Entity.ApplicationUser> userManager,
            SignInManager<Entity.ApplicationUser> signInManager
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
        }


        public override async Task<ResponseDto<LoginResultDto>> HandleCommand(LoginCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<LoginResultDto>();
            var lockoutOnFailure = _configuration.GetValue<bool>("SignInOptions:LockoutEnabled");
            var login = request.LoginDto;

            var user = await _userManager.FindByNameAsync(login.UserName)
               ?? await _userManager.FindByEmailAsync(login.UserName);

            if (user == null)
            {
                response.AddErrorResult("No se pudo iniciar sesión.");
                return response;
            }

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

            var roles = await _userManager.GetRolesAsync(user);
            var applicationCode = login.ApplicationCode?.Trim() ?? string.Empty;
            var negocio = await _mediator!.Send(new GetUsuarioNegocioQuery(user.Id, applicationCode, roles), cancellationToken);
            if (!negocio.IsValid)
            {
                response.AttachResults(negocio);
                return response;
            }

            var accessToken = await _mediator.Send(new GenerateTokenCommand(applicationCode, user, negocio.Data), cancellationToken);

            if (!accessToken.IsValid || accessToken.Data == null)
            {
                response.AttachResults(accessToken);
                response.AddErrorResult("Error al generar token.");
                return response;
            }

            response.UpdateData(new LoginResultDto { AccessToken = accessToken.Data });
            response.AddOkResult("Login exitoso.");
            return response;
        }
    }
}
