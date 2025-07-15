using AutoMapper;
using Google.Apis.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly IRepository<Entity.Models.Usuario> _UsuarioRepository;
        private readonly UserManager<Entity.Models.Usuario> _UsuarioManager;
        private readonly IRepository<Entity.Models.Rol> _RolRepository;
        private readonly IConfiguration _configuration;


        public CreateGoogleCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            //CreateUsuarioCommandValidator validator,
            IRepository<Entity.Models.Usuario> UsuarioRepository,
            UserManager<Entity.Models.Usuario> userManager,
            IRepository<Entity.Models.Rol> RolRepository,
            IConfiguration configuration
        ) : base(unitOfWork, mapper, mediator)
        {
            _UsuarioRepository = UsuarioRepository;
            _UsuarioManager = userManager;
            _configuration = configuration;
            _RolRepository = RolRepository;
        }


        public override async Task<ResponseDto<LoginResultDto>> HandleCommand(CreateGoogleCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<LoginResultDto>();
            GoogleJsonWebSignature.Payload payload;
            try
            {
                // Validamos el token recibido usando la librería oficial de Google
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
            var nuevoUsuario = new Entity.Models.Usuario
            {
                Email = payload.Email,
                Nombre = payload.Name,
                Apellidos = payload.FamilyName,
                Imagen = payload.Picture,
                Telefono = "",
                IdRol = 1,
                IdEstadoUsuario = 1,
                Password = "", // No se usa para Google
                //Proveedor = "Google",
            };

             _UsuarioRepository.UpdateAuditTrails(nuevoUsuario);
             await _UsuarioRepository.SaveAsync();

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
