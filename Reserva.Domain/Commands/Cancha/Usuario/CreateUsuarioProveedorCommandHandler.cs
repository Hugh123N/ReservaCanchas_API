using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Usuario;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reserva.Domain.Commands.Cancha.Usuario
{
    internal class CreateUsuarioProveedorCommandHandler : CommandHandlerBase<CreateUsuarioProveedorCommand, GetUsuarioDto>
    {
        private readonly IRepository<Entity.Models.Usuario> _UsuarioRepository;
        private readonly UserManager<Entity.Models.ApplicationUser> _UsuarioManager;
        private readonly IRepository<Entity.Models.AspNetRole> _RolRepository;
        private readonly IConfiguration _configuration;
        private readonly IRepository<Entity.Models.ApplicationUser> _applicationUserRepository;
        private readonly IRepository<Entity.Models.Proveedor> _ProveedorRepository;
        private readonly IRepository<Entity.Models.EstadoProveedor> _EstadoProveedorRepository;
        private readonly IRepository<Entity.Models.EstadoUsuario> _EstadoUsuarioRepository;

        public CreateUsuarioProveedorCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            IRepository<Entity.Models.Usuario> UsuarioRepository,
            UserManager<Entity.Models.ApplicationUser> userManager,
            IRepository<Entity.Models.AspNetRole> RolRepository,
            IConfiguration configuration,
            IRepository<Entity.Models.ApplicationUser> applicationUserRepository,
            IRepository<Entity.Models.Proveedor> ProveedorRepository,
            IRepository<Entity.Models.EstadoProveedor> EstadoProveedorRepository,
            IRepository<Entity.Models.EstadoUsuario> EstadoUsuarioRepository
        ) : base(unitOfWork, mapper, mediator)
        {
            _UsuarioRepository = UsuarioRepository;
            _UsuarioManager = userManager;
            _configuration = configuration;
            _RolRepository = RolRepository;
            _applicationUserRepository = applicationUserRepository;
            _ProveedorRepository = ProveedorRepository;
            _EstadoProveedorRepository = EstadoProveedorRepository;
            _EstadoUsuarioRepository = EstadoUsuarioRepository;
        }

        public override async Task<ResponseDto<GetUsuarioDto>> HandleCommand(CreateUsuarioProveedorCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetUsuarioDto>();
            var estadoProveedor = await _EstadoProveedorRepository.GetByAsync(x => x.Codigo.Equals(Constants.ESTADO_PROVEEDOR.Pendiente));
            var estadoUsuario = await _EstadoUsuarioRepository.GetByAsync(x => x.Codigo.Equals(Constants.ESTADO_USUARIO.Activo));

            var applicationUser = _mapper?.Map<Entity.Models.ApplicationUser>(request.CreateDto);

            if (applicationUser != null)
            {
                applicationUser.EmailConfirmed = true;
                applicationUser.IdEstadoUsuario = estadoUsuario.IdEstadoUsuario;

                _applicationUserRepository.UpdateAuditTrails(applicationUser);
                var result = await _UsuarioManager.CreateAsync(applicationUser, request.CreateDto.Password);

                if (!result.Succeeded)
                {
                    result.Errors.ToList().ForEach(e =>
                    {
                        response.AddErrorResult($"{e.Code}: {e.Description}");
                    });

                    return response;
                }
                var normalizedRoleNames = new List<string>
                {
                    Constants.Role.Proveedor.ToUpper(),
                    Constants.Role.Cliente.ToUpper(),
                    Constants.Role.Operador.ToUpper()
                };

                var roles = await _RolRepository.FindByAsync(x => normalizedRoleNames.Contains(x.NormalizedName!));

                if (roles.Any())
                {
                    var roleResult = await _UsuarioManager.AddToRolesAsync(applicationUser, roles.Select(x => x.NormalizedName));
                    if (!roleResult.Succeeded)
                    {
                        roleResult.Errors.ToList().ForEach(e =>
                        {
                            response.AddErrorResult($"Error al asignar rol: {e.Code}: {e.Description}");
                        });
                        await _UsuarioManager.DeleteAsync(applicationUser);
                        return response;
                    }
                }
                else
                {
                    response.AddErrorResult("Rol no encontrado o inválido.");
                    await _UsuarioManager.DeleteAsync(applicationUser);
                    return response;
                }

                var proveedor = _mapper?.Map<Entity.Models.Proveedor>(request.CreateDto);
                if (proveedor != null)
                {
                    proveedor.IdProveedor = applicationUser.Id;
                    proveedor.IdEstadoProveedor = estadoProveedor.IdEstadoProveedor;

                    try
                    {
                        await _ProveedorRepository.AddAsync(proveedor);
                        await _ProveedorRepository.SaveAsync();
                    }
                    catch (Exception ex)
                    {
                        response.AddErrorResult($"Error al crear proveedor: {ex.Message}");
                        await _UsuarioManager.DeleteAsync(applicationUser);
                        return response;
                    }
                }
            }

            var UsuarioDto = _mapper?.Map<GetUsuarioDto>(applicationUser);
            if (UsuarioDto != null) response.UpdateData(UsuarioDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    
    }
}
