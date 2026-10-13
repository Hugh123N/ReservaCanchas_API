using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Domain.Commands.Dbo.Usuario
{
    public class ChangeEmailConfirmCommandHandler : CommandHandlerBase<ChangeEmailConfirmCommand>
    {
        private readonly UserManager<Entity.ApplicationUser> _userManager;
        private readonly IRepository<Entity.AspNetUsers> _usuarioRepository;
        private readonly IRepository<Entity.Proveedor> _proveedorRepository;
        private readonly IRepository<Entity.Operador> _operadorRepository;

        public ChangeEmailConfirmCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            ChangeEmailConfirmCommandValidator validator,
            UserManager<Entity.ApplicationUser> userManager,
            IRepository<Entity.AspNetUsers> usuarioRepository,
            IRepository<Entity.Proveedor> proveedorRepository,
            IRepository<Entity.Operador> operadorRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _userManager = userManager;
            _usuarioRepository = usuarioRepository;
            _proveedorRepository = proveedorRepository;
            _operadorRepository = operadorRepository;
        }

        public override async Task<ResponseDto> HandleCommand(ChangeEmailConfirmCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();

            string decodedToken;
            try
            {
                decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
            }
            catch (Exception)
            {
                response.AddErrorResult("El enlace de confirmación es inválido o ha expirado.");
                return response;
            }

            var newEmail = request.Email.Trim();

            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
            {
                response.AddErrorResult("Usuario no encontrado.");
                return response;
            }

            var isValidToken = await _userManager.VerifyUserTokenAsync(
                user, TokenOptions.DefaultProvider, GetChangeEmailPurpose(newEmail), decodedToken);
            if (!isValidToken)
            {
                response.AddErrorResult("El enlace de confirmación es inválido o ha expirado.");
                return response;
            }

            // Revalida disponibilidad al momento de aplicar por si el correo se ocupó entre el envío y la confirmación
            var existingByEmail = await _userManager.FindByEmailAsync(newEmail);
            if (existingByEmail != null && existingByEmail.Id != user.Id)
            {
                response.AddErrorResult("El correo ya está registrado en otra cuenta.");
                return response;
            }

            var existingByUserName = await _userManager.FindByNameAsync(newEmail);
            if (existingByUserName != null && existingByUserName.Id != user.Id)
            {
                response.AddErrorResult("El correo ya está registrado en otra cuenta.");
                return response;
            }

            var usuario = await _usuarioRepository.GetByAsync(x => x.Id == request.UserId);
            if (usuario == null)
            {
                response.AddErrorResult("Usuario no encontrado.");
                return response;
            }

            // Si el UserName coincide con el correo anterior se actualiza junto con el correo,
            // de lo contrario el usuario tendría que seguir entrando con el correo viejo
            var actualizarUserName = usuario.NormalizedUserName != null
                && string.Equals(usuario.NormalizedUserName, usuario.NormalizedEmail, StringComparison.OrdinalIgnoreCase);

            usuario.Email = newEmail;
            usuario.NormalizedEmail = _userManager.NormalizeEmail(newEmail);
            usuario.EmailConfirmed = true;

            if (actualizarUserName)
            {
                usuario.UserName = newEmail;
                usuario.NormalizedUserName = _userManager.NormalizeName(newEmail);
            }

            await _usuarioRepository.UpdateAsync(usuario);

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Any(r => string.Equals(r, Constants.Role.Proveedor, StringComparison.OrdinalIgnoreCase)))
            {
                var proveedor = await _proveedorRepository.GetByAsync(x => x.IdUsuario == request.UserId.ToString());
                if (proveedor != null)
                {
                    proveedor.Email = newEmail;
                    await _proveedorRepository.UpdateAsync(proveedor);
                }
            }
            else if (roles.Any(r => string.Equals(r, Constants.Role.Operador, StringComparison.OrdinalIgnoreCase)))
            {
                var operador = await _operadorRepository.GetByAsync(x => x.IdUsuario == request.UserId.ToString());
                if (operador != null)
                {
                    operador.Email = newEmail;
                    await _operadorRepository.UpdateAsync(operador);
                }
            }

            response.AddOkResult("Correo actualizado correctamente.");
            return response;
        }

        // Debe coincidir con el purpose usado al generar el token en ChangeEmailRequestCommandHandler
        private static string GetChangeEmailPurpose(string newEmail)
            => $"ChangeEmail:{newEmail}";
    }
}
