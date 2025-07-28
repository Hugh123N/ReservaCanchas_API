using AutoMapper;
using Google.Apis.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Commands.Token;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Usuario;
using Reserva.Dto.User;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Domain.Commands.Cancha.Usuario
{
    internal class CreateGoogleCommandHandler : CommandHandlerBase<CreateGoogleCommand, LoginResultDto>
    {
        private readonly IRepository<Entity.Models.AspNetUser> _UsuarioRepository;
        private readonly UserManager<Entity.Models.ApplicationUser> _UsuarioManager;
        private readonly IRepository<Entity.Models.Rol> _RolRepository;
        private readonly IConfiguration _configuration;
        private readonly IRepository<Entity.Models.ApplicationUser> _applicationUserRepository;
        private readonly IRepository<Entity.Models.EstadoUsuario> _EstadoUsuarioRepository;

        public CreateGoogleCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            //CreateUsuarioCommandValidator validator,
            IRepository<Entity.Models.AspNetUser> UsuarioRepository,
            UserManager<Entity.Models.ApplicationUser> userManager,
            IRepository<Entity.Models.ApplicationUser> ApplicationUserRepository,
            IRepository<Entity.Models.Rol> RolRepository,
            IRepository<Entity.Models.EstadoUsuario> EstadoUsuarioRepository,
        IConfiguration configuration
        ) : base(unitOfWork, mapper, mediator)
        {
            _UsuarioRepository = UsuarioRepository;
            _UsuarioManager = userManager;
            _configuration = configuration;
            _RolRepository = RolRepository;
            _applicationUserRepository = ApplicationUserRepository;
            _EstadoUsuarioRepository = EstadoUsuarioRepository;
        }


        public override async Task<ResponseDto<LoginResultDto>> HandleCommand(CreateGoogleCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<LoginResultDto>();
            var estadoUsuario = await _EstadoUsuarioRepository.GetByAsNoTrackingAsync(x => x.Codigo.Equals(Constants.ESTADO_USUARIO.Activo));
            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(request.CreateDto.IdToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    // En producción: valida el client_id también
                    // Audience = new[] { "TU_GOOGLE_CLIENT_ID" }
                });
            }
            catch
            {
                response.AddErrorResult("Token inválido o expirado.");
                return response;
            }

            var usuarioExistente = await _UsuarioRepository.GetByAsync(u => u.Email == payload.Email);
            if (usuarioExistente != null)
            {
                response.AddErrorResult("Este correo ya está registrado.");
                return response;
            }

            // Crear nuevo usuario con los datos de Google
            var nuevoUsuario = new Entity.Models.ApplicationUser
            {
                Email = payload.Email,
                UserName = payload.Name,
                LastName = payload.FamilyName,
                IdEstadoUsuario = estadoUsuario!.IdEstadoUsuario,
                Imagen = payload.Picture
                //PhoneNumber = "",
               // IdRol = 1,
               // Password = "", // No se usa para Google
            };

            _applicationUserRepository.UpdateAuditTrails(nuevoUsuario);
             await _UsuarioManager.CreateAsync(nuevoUsuario, "");

            var accessToken = await _mediator.Send(new GenerateTokenCommand(nuevoUsuario), cancellationToken)!;

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
