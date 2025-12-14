using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Usuario;
using Reserva.Entity;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Usuario
{
    public class CreateUsuarioCommandHandler : CommandHandlerBase<CreateUsuarioCommand, GetUsuarioDto>
    {
        private readonly UserManager<Entity.ApplicationUser> _UsuarioManager;
        private readonly IRepository<Entity.AspNetRoles> _RolRepository;
        private readonly IConfiguration _configuration;
        private readonly IRepository<Entity.ApplicationUser> _applicationUserRepository;
        private readonly IRepository<Entity.Proveedor> _ProveedorRepository;
        private readonly IRepository<Entity.EstadoUsuario> _EstadoUsuarioRepository;


        public CreateUsuarioCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateUsuarioCommandValidator validator,
            UserManager<Entity.ApplicationUser> userManager,
            IRepository<Entity.AspNetRoles> RolRepository,
            IConfiguration configuration,
            IRepository<Entity.ApplicationUser> applicationUserRepository,
            IRepository<Entity.Proveedor> ProveedorRepository,
            IRepository<Entity.EstadoUsuario> EstadoUsuarioRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _UsuarioManager = userManager;
            _configuration = configuration;
            _RolRepository = RolRepository;
            _applicationUserRepository = applicationUserRepository;
            _ProveedorRepository = ProveedorRepository;
            _EstadoUsuarioRepository = EstadoUsuarioRepository;
        }

        public override async Task<ResponseDto<GetUsuarioDto>> HandleCommand(CreateUsuarioCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetUsuarioDto>();
            var result = new IdentityResult();
            var estadoUsuario = await _EstadoUsuarioRepository.GetByAsync(x => x.Codigo.Equals(Constants.ESTADO_USUARIO.Activo));
             
            var applicationUser = _mapper?.Map<Entity.ApplicationUser>(request.CreateDto);

            applicationUser.EmailConfirmed = true;
            applicationUser.IdEstadoUsuario = estadoUsuario!.IdEstadoUsuario;

            _applicationUserRepository.UpdateAuditTrails(applicationUser);
            if (request.CreateDto.Password != null) {
                result = await _UsuarioManager.CreateAsync(applicationUser, request.CreateDto.Password);
            }else{
                result = await _UsuarioManager.CreateAsync(applicationUser);
            }

            if (!result.Succeeded)
            {
                result.Errors.ToList().ForEach(e =>
                {
                    response.AddErrorResult($"{e.Code}: {e.Description}");
                });

                return response;
            }

            var rols = await _RolRepository.FindByAsNoTrackingAsync(x => x.Activo);

            var roleIds = request.CreateDto.RoleIds ?? new List<Guid>();
            var roles = rols.Where(x => roleIds.Contains(x.Id));

            if (roles.Any())
            {
                var addRolesResult = await _UsuarioManager.AddToRolesAsync(applicationUser, roles.Select(x => x.NormalizedName));
                if (!addRolesResult.Succeeded)
                    addRolesResult.Errors.ToList().ForEach(e => { response.AddErrorResult($"{e.Code}: {e.Description}"); });
            }
            if (request.CreateDto.Host != null && request.CreateDto.Password == null) {
                try
                {
                    await _mediator!.Send(new ForgotPasswordCommand(request.CreateDto.Email!, request.CreateDto.Host), cancellationToken);
                }
                catch (Exception ex)
                {
                    response.AddWarningResult("Error al enviar Email de forgot password");
                }
            }
            
            var UsuarioDto = _mapper?.Map<GetUsuarioDto>(applicationUser);
            if (UsuarioDto != null) response.UpdateData(UsuarioDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }

    }
}