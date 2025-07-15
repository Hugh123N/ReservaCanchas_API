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
        private readonly UserManager<Entity.Models.Usuario> _userManager;
        private readonly SignInManager<Entity.Models.Usuario> _signInManager;
        private readonly IRepository<Entity.Models.Rol> _rolRepository;
        

        public LoginCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            LoginCommandValidator validator,
            IConfiguration configuration,
            IRepository<Entity.Models.Usuario> usuarioRepository,
            UserManager<Entity.Models.Usuario> userManager,
            SignInManager<Entity.Models.Usuario> signInManager,
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

            var user = await _userManager.FindByNameAsync(request.LoginDto.UserName);
            user ??= await _userManager.FindByEmailAsync(request.LoginDto.UserName);

            var result = await _signInManager.PasswordSignInAsync(user.Nombre, request.LoginDto.Password, request.LoginDto.RememberMe, lockoutOnFailure: lockoutOnFailure);

            if (result.Succeeded)
            {
                var accessToken = await _mediator.Send(new GenerateTokenCommand(user), cancellationToken)!;

                if (accessToken?.Data == null)
                {
                    response.AddErrorResult("Error al generar token.");
                    return response;
                }

                response.UpdateData(new LoginResultDto { AccessToken = accessToken.Data });
                response.AddOkResult("Login exitoso.");
            }
            return response;
        }
    }
}
