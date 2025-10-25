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
            var estadoUsuario = await _EstadoUsuarioRepository.GetByAsync(x => x.Codigo.Equals(Constants.ESTADO_USUARIO.Activo));
             
            var applicationUser = _mapper?.Map<Entity.ApplicationUser>(request.CreateDto);

            if (applicationUser != null)
            {
                applicationUser.EmailConfirmed = true;
                applicationUser.IdEstadoUsuario = estadoUsuario!.IdEstadoUsuario;

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

                var role = await _RolRepository.GetByAsync(x => x.NormalizedName.Equals(Constants.Role.Cliente));

                if (role != null)
                {
                    var roleResult = await _UsuarioManager.AddToRoleAsync(applicationUser, role.NormalizedName);
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

                /*if (roles.Any(x => x.NormalizedName.Equals(Constants.Role.Proveedor)))
                {
                    var proveedor = _mapper?.Map<Entity.Proveedor>(request.CreateDto);
                    if (proveedor != null)
                    {
                        proveedor.IdProveedor = applicationUser.Id;

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
                }*/
            }

            var UsuarioDto = _mapper?.Map<GetUsuarioDto>(applicationUser);
            if (UsuarioDto != null) response.UpdateData(UsuarioDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }

        /*public async Task SendCreationEmail(CreateUsuarioCommand request)
        {
            var sendMail = _configuration.GetValue<bool>("SignInOptions:SendMailOnSignUp");
            if (sendMail)
            {
                var application = _configuration.GetValue<string>("ApiOptions:Name");
                var frontUrlLogo = _configuration.GetValue<string>("SecurityOptions:FrontUrlLogo");

                var emailDto = new SendEmailDto
                {
                    EmailCode = Constants.Email.User.Registration,
                    ToEmails = new List<string> { request.CreateDto?.Email ?? string.Empty },
                    BodyParams = new Dictionary<string, string>
                    {
                        { "{APPLICATION}", application },
                        { "{LOGO}", frontUrlLogo },
                        { "{USER}", request.CreateDto?.UserName! },
                        { "{PASSWORD}", request.CreateDto?.Password! }
                    }
                };

                await _mediator!.Send(new SendEmailCommand(emailDto));
            }
        }*/
    }
}