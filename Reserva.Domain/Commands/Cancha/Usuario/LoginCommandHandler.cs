using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
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
        private readonly IRepository<Entity.Models.Usuario> _usuarioRepository;
        private readonly UserManager<Entity.Models.ApplicationUser> _userManager;
        private readonly SignInManager<Entity.Models.ApplicationUser> _signInManager;
        private readonly IRepository<Entity.Models.Rol> _rolRepository;
        

        public LoginCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            LoginCommandValidator validator,
            IConfiguration configuration,
            IRepository<Entity.Models.Usuario> usuarioRepository,
            UserManager<Entity.Models.ApplicationUser> userManager,
            SignInManager<Entity.Models.ApplicationUser> signInManager,
        IRepository<Entity.Models.Rol> rolRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _configuration = configuration;
            _usuarioRepository = usuarioRepository;
            _rolRepository = rolRepository;
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

            var accessToken = await _mediator.Send(new GenerateTokenCommand(request.LoginDto.ApplicationCode, user), cancellationToken)!;

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
