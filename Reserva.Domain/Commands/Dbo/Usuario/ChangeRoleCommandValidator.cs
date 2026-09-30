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
    public class ChangeRoleCommandValidator : CommandValidatorBase<ChangeRoleCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<ApplicationRole> _rolRepository;
        private readonly IRepository<Entity.Proveedor> _proveedorRepository;
        private readonly IRepository<Entity.Operador> _operadorRepository;

        public ChangeRoleCommandValidator(
            UserManager<ApplicationUser> userManager,
            IRepository<ApplicationRole> rolRepository,
            IRepository<Entity.Proveedor> proveedorRepository,
            IRepository<Entity.Operador> operadorRepository)
        {
            _userManager = userManager;
            _rolRepository = rolRepository;
            _proveedorRepository = proveedorRepository;
            _operadorRepository = operadorRepository;

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("El ID de usuario es obligatorio.")
                .MustAsync(UsuarioExiste).WithMessage("Usuario no encontrado.")
                .MustAsync(NoEsAdministrador).WithMessage("Un usuario administrador no puede cambiar de rol.");

            RuleFor(x => x.ChangeRoleDto.TargetRole)
                .NotEmpty().WithMessage("El rol destino es obligatorio.")
                .Must(EsRolValido).WithMessage("El rol especificado no es válido.");

            RuleFor(x => x)
                .MustAsync(ExistenRolesRequeridos).WithMessage("Los roles requeridos no están disponibles.");

            RuleFor(x => x.ChangeRoleDto.Ruc)
                .Length(11).WithMessage("El RUC debe tener 11 dígitos.")
                .Matches("^[0-9]+$").WithMessage("El RUC solo debe contener números.")
                .When(x => x.ChangeRoleDto.TargetRole.Equals(Constants.Role.Proveedor, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(x.ChangeRoleDto.Ruc));

            RuleFor(x => x.UserId)
                .MustAsync((userId, cancellationToken) => 
                    NoExisteYaConEseRol(userId, null, cancellationToken))
                .WithMessage("Este usuario ya tiene el rol solicitado.")
                .When(x => x.ChangeRoleDto.TargetRole.Equals(Constants.Role.Proveedor, StringComparison.OrdinalIgnoreCase) ||
                          x.ChangeRoleDto.TargetRole.Equals(Constants.Role.Operador, StringComparison.OrdinalIgnoreCase));
        }

        private async Task<bool> UsuarioExiste(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user != null;
        }

        private bool EsRolValido(string targetRole)
        {
            var validRoles = new[] { Constants.Role.Proveedor, Constants.Role.Operador, Constants.Role.Cliente };
            return validRoles.Any(r => r.Equals(targetRole, StringComparison.OrdinalIgnoreCase));
        }

        private async Task<bool> ExistenRolesRequeridos(ChangeRoleCommand command, CancellationToken cancellationToken)
        {
            var normalizedRoles = new List<string>
            {
                Constants.Role.Proveedor.ToUpper(),
                Constants.Role.Operador.ToUpper(),
                Constants.Role.Cliente.ToUpper()
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

        private async Task<bool> NoExisteYaConEseRol(Guid userId, ChangeRoleCommand? command, CancellationToken cancellationToken)
        {
            var targetRole = command?.ChangeRoleDto.TargetRole ?? "";
            
            if (targetRole.Equals(Constants.Role.Proveedor, StringComparison.OrdinalIgnoreCase))
            {
                var proveedor = await _proveedorRepository.GetByAsync(p => p.IdUsuario == userId);
                return proveedor == null;
            }
            else if (targetRole.Equals(Constants.Role.Operador, StringComparison.OrdinalIgnoreCase))
            {
                var operador = await _operadorRepository.GetByAsync(p => p.IdUsuario == userId);
                return operador == null;
            }
            
            return true;
        }
    }
}