using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Reserva.Common;
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
    public class UpgradeToProveedorCommandValidator : CommandValidatorBase<UpgradeToProveedorCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<ApplicationRole> _rolRepository;
        private readonly IRepository<Entity.Proveedor> _proveedorRepository;

        public UpgradeToProveedorCommandValidator(
            UserManager<ApplicationUser> userManager,
            IRepository<ApplicationRole> rolRepository,
            IRepository<Entity.Proveedor> proveedorRepository)
        {
            _userManager = userManager;
            _rolRepository = rolRepository;
            _proveedorRepository = proveedorRepository;

            RuleFor(x => x.UpgradeDto.Ruc)
                .Length(11).WithMessage("El RUC debe tener 11 dígitos.")
                .Matches("^[0-9]+$").WithMessage("El RUC solo debe contener números.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("El ID de usuario es obligatorio.")
                .MustAsync(UsuarioExiste).WithMessage("Usuario no encontrado.")
                .MustAsync(UsuarioNoEsYaProveedor).WithMessage("El usuario ya es un proveedor.");

            RuleFor(x => x.UpgradeDto)
                .NotNull().WithMessage("La información del proveedor es obligatoria.");

            RuleFor(x => x)
                .MustAsync(ExistenRolesProveedorYOperador).WithMessage("Los roles requeridos no están disponibles.");

            RuleFor(x => x.UserId)
                .MustAsync(NoEsAdministrador)
                .WithMessage("Un usuario administrador no puede convertirse en proveedor.");

            RuleFor(x => x.UserId)
                .MustAsync(NoExisteYaComoProveedor)
                .WithMessage("Este usuario ya está registrado como proveedor.");
            
        }

        private async Task<bool> UsuarioExiste(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user != null;
        }

        private async Task<bool> UsuarioNoEsYaProveedor(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return true; // ya validado en la anterior
            return !await _userManager.IsInRoleAsync(user, Constants.Role.Proveedor);
        }

        private async Task<bool> ExistenRolesProveedorYOperador(UpgradeToProveedorCommand command, CancellationToken cancellationToken)
        {
            var normalizedRoles = new List<string>
        {
            Constants.Role.Proveedor.ToUpper(),
            Constants.Role.Operador.ToUpper()
        };

            var roles = await _rolRepository.FindByAsync(x => normalizedRoles.Contains(x.NormalizedName!));
            return roles.Any();
        }

        private async Task<bool> NoEsAdministrador(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return true;
            var roles = await _userManager.GetRolesAsync(user);
            return !roles.Contains(Constants.Role.Admin);
        }

        private async Task<bool> NoExisteYaComoProveedor(Guid userId, CancellationToken cancellationToken)
        {
            var proveedor = await _proveedorRepository.GetByAsync(p => p.IdProveedor == userId);
            return proveedor == null;
        }
    }
}
