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
using Reserva.Dto.Cancha.Usuario;
using Reserva.Dto.User;
using Reserva.Entity.Models;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Domain.Commands.Cancha.Usuario
{
    internal class CreateAndLoginCommandHandler : CommandHandlerBase<CreateAndLoginCommand, LoginResultDto>
    {
        private readonly IRepository<Entity.Models.AspNetUser> _UsuarioRepository;
        private readonly UserManager<Entity.Models.ApplicationUser> _UserManager;
        private readonly SignInManager<Entity.Models.ApplicationUser> _SignInManager;
        private readonly IRepository<Entity.Models.AspNetRole> _RolRepository;
        private readonly IConfiguration _configuration;
        private readonly IRepository<Entity.Models.ApplicationUser> _applicationUserRepository;
        private readonly IRepository<Entity.Models.EstadoUsuario> _EstadoUsuarioRepository;


        public CreateAndLoginCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            //CreateUsuarioCommandValidator validator,
            IRepository<Entity.Models.AspNetUser> UsuarioRepository,
            UserManager<Entity.Models.ApplicationUser> userManager,
            IRepository<Entity.Models.ApplicationUser> ApplicationUserRepository,
            IRepository<Entity.Models.AspNetRole> RolRepository,
            IRepository<Entity.Models.EstadoUsuario> EstadoUsuarioRepository,
        IConfiguration configuration
        ) : base(unitOfWork, mapper, mediator)
        {
            _UsuarioRepository = UsuarioRepository;
            _UserManager = userManager;
            _configuration = configuration;
            _RolRepository = RolRepository;
            _applicationUserRepository = ApplicationUserRepository;
            _EstadoUsuarioRepository = EstadoUsuarioRepository;
        }


        public override async Task<ResponseDto<LoginResultDto>> HandleCommand(CreateAndLoginCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<LoginResultDto>();
            var nuevoUsuario = new Entity.Models.ApplicationUser();
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

            var accessToken = await _mediator.Send(new GenerateTokenCommand(request.CreateDto.ApplicationCode, nuevoUsuario), cancellationToken)!;

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
