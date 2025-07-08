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

namespace Reserva.Domain.Commands.User
{
    public class LoginCommandHandler : CommandHandlerBase<LoginCommand, LoginResultDto>
    {
        private readonly IConfiguration _configuration;
        private readonly IRepository<Entity.Models.Usuario> _usuarioRepository;
        private readonly IRepository<Entity.Models.Rol> _rolRepository;
        

        public LoginCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            LoginCommandValidator validator,
            IConfiguration configuration,
            IRepository<Entity.Models.Usuario> usuarioRepository,
            IRepository<Entity.Models.Rol> rolRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _configuration = configuration;
            _usuarioRepository = usuarioRepository;
            _rolRepository = rolRepository;
        }


        public override async Task<ResponseDto<LoginResultDto>> HandleCommand(LoginCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<LoginResultDto>();
            var login = request.LoginDto;

            var user = await _usuarioRepository.GetByAsync(
                x => x.Email == login.UserName || x.Nombre == login.UserName && x.Activo);

            if (user == null || !VerifyPassword(user.Password, login.Password!))
            {
                response.AddErrorResult("Usuario o contraseña incorrectos.");
                return response;
            }

            var accessToken = await _mediator.Send(new GenerateTokenCommand(login.ApplicationCode, user), cancellationToken);

            if (accessToken?.Data == null)
            {
                response.AddErrorResult("Error al generar token.");
                return response;
            }

            response.UpdateData(new LoginResultDto { AccessToken = accessToken.Data });
            response.AddOkResult("Login exitoso.");
            return response;
        }

        private bool VerifyPassword(string? hashedPassword, string plainPassword)
        {
            // Reemplaza esto por lógica real de hashing como BCrypt
            return hashedPassword == GenerateTokenCommandHandler.EncriptarSHA256(plainPassword);
        }
    }
}
