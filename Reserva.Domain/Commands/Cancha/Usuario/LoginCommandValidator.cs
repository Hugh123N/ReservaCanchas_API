using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.User;
using Reserva.Entity;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Commands.User
{
    public class LoginCommandValidator : CommandValidatorBase<LoginCommand>
    {
        UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LoginCommandValidator(
             UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager
        )
        {

            _userManager = userManager;
            _signInManager = signInManager;

            RequiredInformation(x => x.LoginDto).DependentRules(() =>
            {
                RuleFor(x => x.LoginDto.UserName)
                .NotEmpty().WithMessage("El nombre de usuario o email es requerido.")
                .MaximumLength(256).WithMessage("El nombre de usuario no puede superar los 256 caracteres.");

                RuleFor(x => x.LoginDto.Password)
                    .NotEmpty().WithMessage("La contraseña es requerida.")
                    .MaximumLength(256).WithMessage("La contraseña no puede superar los 256 caracteres.");

                RuleFor(x => x.LoginDto)
                .MustAsync(UsuarioExisteYActivo)
                .WithMessage("El usuario no existe o está inactivo.");

                RuleFor(x => x.LoginDto)
                    .MustAsync(PasswordEsValida)
                    .WithMessage("Credenciales inválidas.");

                RuleFor(x => x.LoginDto)
                    .MustAsync(UsuarioNoSuspendido)
                    .WithMessage("Tu cuenta ha sido suspendida. Por favor, contacta al soporte.");
            });
        }

        private async Task<bool> UsuarioExisteYActivo(LoginDto dto, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName)
                       ?? await _userManager.FindByEmailAsync(dto.UserName);

            return user != null && user.IdEstadoUsuario != 3; 
        }

        private async Task<bool> PasswordEsValida(LoginDto dto, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName)
                       ?? await _userManager.FindByEmailAsync(dto.UserName);

            if (user == null) return false;

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);
            return result.Succeeded;
        }

        private async Task<bool> UsuarioNoSuspendido(LoginDto dto, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName)
                       ?? await _userManager.FindByEmailAsync(dto.UserName);

            return user == null || user.IdEstadoUsuario != 3;
        }
    }
}
