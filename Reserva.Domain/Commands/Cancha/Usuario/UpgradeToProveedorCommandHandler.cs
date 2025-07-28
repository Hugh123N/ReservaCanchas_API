using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Usuario;
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
    public class UpgradeToProveedorCommandHandler : CommandHandlerBase<UpgradeToProveedorCommand, GetUsuarioDto>
    {
        private readonly UserManager<ApplicationUser> _UsuarioManager;
        private readonly IRepository<ApplicationRole> _RolRepository;
        private readonly IRepository<Entity.Models.Proveedor> _ProveedorRepository;
        private readonly Entity.Models.ReservaCanchasContext _dbContext;

        public UpgradeToProveedorCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            IRepository<ApplicationRole> rolRepository,
            IRepository<Entity.Models.Proveedor> proveedorRepository,
            ReservaCanchasContext dbContext
        ) : base(unitOfWork, mapper)
        {
            _UsuarioManager = userManager;
            _RolRepository = rolRepository;
            _ProveedorRepository = proveedorRepository;
            _dbContext = dbContext;
        }

        public override async Task<ResponseDto<GetUsuarioDto>> HandleCommand(UpgradeToProveedorCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetUsuarioDto>();

            var applicationUser = await _UsuarioManager.FindByIdAsync(request.UserId.ToString());
            if (applicationUser == null)
            {
                response.AddErrorResult("Usuario no encontrado.");
                return response;
            }

            var isProveedor = await _UsuarioManager.IsInRoleAsync(applicationUser, Constants.Role.Proveedor);
            if (isProveedor)
            {
                response.AddErrorResult("El usuario ya es un proveedor.");
                return response;
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var proveedorRole = await _RolRepository.GetByAsync(x => x.NormalizedName.Equals(Constants.Role.Proveedor));
                if (proveedorRole == null)
                {
                    response.AddErrorResult("El rol 'Proveedor' no existe en el sistema.");
                    transaction?.Rollback();
                    return response;
                }

                var addRoleResult = await _UsuarioManager.AddToRoleAsync(applicationUser, proveedorRole.NormalizedName!);
                if (!addRoleResult.Succeeded)
                {
                    addRoleResult.Errors.ToList().ForEach(e => response.AddErrorResult($"Error al asignar rol de Proveedor: {e.Code}: {e.Description}"));
                    transaction?.Rollback();
                    return response;
                }

                var proveedor = _mapper?.Map<Entity.Models.Proveedor>(request.UpgradeDto);
                if (proveedor == null)
                {
                    response.AddErrorResult("No se pudo mapear la información del proveedor.");
                    await _UsuarioManager.RemoveFromRoleAsync(applicationUser, proveedorRole.NormalizedName!); // Rollback del rol
                    transaction?.Rollback();
                    return response;
                }

                proveedor.IdProveedor = applicationUser.Id;
                proveedor.IdEstadoProveedor = 1;

                await _ProveedorRepository.AddAsync(proveedor);
                await _ProveedorRepository.SaveAsync();

                // Si todo salió bien, confirmar la transacción
                transaction?.Commit();

                response.AddOkResult(Resources.Common.UpdateSuccessMessage);
            }
            catch (Exception ex)
            {
                transaction?.Rollback(); 
                await _UsuarioManager.RemoveFromRoleAsync(applicationUser, Constants.Role.Proveedor); // Si ya se asignó el rol
                response.AddErrorResult($"Error al actualizar a proveedor: {ex.Message}");
                return response;
            }

            var UsuarioDto = _mapper?.Map<GetUsuarioDto>(applicationUser);
            if (UsuarioDto != null) response.UpdateData(UsuarioDto);

            return await Task.FromResult(response);
        }
    }
}
