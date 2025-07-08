using FluentValidation;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.User;
using Reserva.Repository.Abstractions.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Reserva.Domain.Commands.User
{
    public class LoginCommandValidator : CommandValidatorBase<LoginCommand>
    {
        private readonly IRepository<Entity.Models.Usuario> _usuarioRepository;

        public LoginCommandValidator(
            IRepository<Entity.Models.Usuario> usuarioRepository
        )
        {
            _usuarioRepository = usuarioRepository;

            RequiredInformation(x => x.LoginDto).DependentRules(() =>
            {
                RequiredString(x => x.LoginDto.UserName, "El nombre de usuario o email es requerido.", default, 256);
                RequiredString(x => x.LoginDto.Password, "La contraseña es requerida.", default, 256);

                RuleFor(x => x.LoginDto.UserName)
                    .MustAsync(ValidateUsuarioActivo)
                    .WithMessage("El usuario no existe o está inactivo.");
            });
        }

        private async Task<bool> ValidateUsuarioActivo(string userName, CancellationToken cancellationToken)
        {
            var usuario = await _usuarioRepository
                .FindByAsNoTrackingAsync(x =>
                    x.Activo &&
                    (x.Email == userName || x.Nombre == userName)
                );

            return usuario != null;
        }
    }
}
