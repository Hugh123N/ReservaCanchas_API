using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Reserva.Domain.Commands.Base;
using Reserva.Entity;
using Reserva.Repository.Abstractions.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Domain.Commands.Dbo.Usuario
{
    public class CreateUsuarioProveedorValidator : CommandValidatorBase<CreateUsuarioProveedorCommand>
    {
        public CreateUsuarioProveedorValidator(
            UserManager<ApplicationUser> applicationUserRepository,
            IRepository<AspNetUsers> usuarioRepository,
            IRepository<Entity.Proveedor> proveedorRepository)
        {
            RuleFor(x => x.CreateDto.UserName)
                .NotEmpty().WithMessage("El nombre de usuario es obligatorio.")
                .MaximumLength(50).WithMessage("Máximo 50 caracteres.")
                .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$").WithMessage("Solo se permiten letras y espacios.");

            RuleFor(x => x.CreateDto.Email)
                .NotEmpty().WithMessage("El email es obligatorio.")
                .EmailAddress().WithMessage("El email no tiene formato válido.")
                .MustAsync(async (email, _) =>
                    await applicationUserRepository.FindByEmailAsync(email) is null)
                .WithMessage("El email ya está registrado.");

            RuleFor(x => x.CreateDto.PhoneNumber)
                .Matches(@"^\d{9}$").WithMessage("El número debe tener 9 dígitos.")
                .MustAsync(async (phone, _) =>
                    await applicationUserRepository.FindByLoginAsync("PhoneNumber", phone) is null)
                .WithMessage("El teléfono ya está registrado.");

            RuleFor(x => x.CreateDto.FirstName)
                .MaximumLength(100).WithMessage("Máximo 100 caracteres.")
                .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$").WithMessage("Solo letras y espacios.");

            RuleFor(x => x.CreateDto.LastName)
                .MaximumLength(100).WithMessage("Máximo 100 caracteres.")
                .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$").WithMessage("Solo letras y espacios.");

            RuleFor(x => x.CreateDto.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(6).WithMessage("Debe tener al menos 6 caracteres.");

            RuleFor(x => x.CreateDto.ConfirmPassword)
                .Equal(x => x.CreateDto.Password).WithMessage("Las contraseñas no coinciden.");

            RuleFor(x => x.CreateDto.RazonSocial)
                .MaximumLength(150).WithMessage("Máximo 150 caracteres.")
                .MustAsync(async (razonSolcial, _) =>
                    await proveedorRepository.GetByAsync(x => x.RazonSocial == razonSolcial) == null)
                .WithMessage("El Razon Social ya está registrado.");

            RuleFor(x => x.CreateDto.Ruc)
                .Matches(@"^\d{11}$").WithMessage("El RUC debe tener exactamente 11 dígitos.")
                .MustAsync(async (ruc, _) =>
                    await proveedorRepository.GetByAsync(x => x.Ruc == ruc) == null)
                .WithMessage("El RUC ya está registrado.");

            RuleFor(x => x.CreateDto.IdTipoProveedor)
                .GreaterThan(0).WithMessage("Debe seleccionar un tipo de proveedor válido.");
        }
    }
}

