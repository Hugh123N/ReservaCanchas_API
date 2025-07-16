using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Usuario;
using Reserva.Entity.Models;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Usuario
{
    public class CreateUsuarioCommandHandler : CommandHandlerBase<CreateUsuarioCommand, GetUsuarioDto>
    {
        private readonly IRepository<Entity.Models.Usuario> _UsuarioRepository;
        private readonly UserManager<Entity.Models.ApplicationUser> _UsuarioManager;
        private readonly IRepository<Entity.Models.Rol> _RolRepository;
        private readonly IConfiguration _configuration;
        private readonly IRepository<Entity.Models.ApplicationUser> _applicationUserRepository;


        public CreateUsuarioCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateUsuarioCommandValidator validator,
            IRepository<Entity.Models.Usuario> UsuarioRepository,
            UserManager<Entity.Models.ApplicationUser> userManager,
            IRepository<Entity.Models.Rol> RolRepository,
            IConfiguration configuration,
            IRepository<Entity.Models.ApplicationUser> applicationUserRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _UsuarioRepository = UsuarioRepository;
            _UsuarioManager = userManager;
            _configuration = configuration;
            _RolRepository = RolRepository;
            _applicationUserRepository = applicationUserRepository;
        }

        public override async Task<ResponseDto<GetUsuarioDto>> HandleCommand(CreateUsuarioCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetUsuarioDto>();

            var applicationUser = _mapper?.Map<Entity.Models.ApplicationUser>(request.CreateDto);

            if (applicationUser != null)
            {
                applicationUser.EmailConfirmed = true;

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

                if (response.IsValid)
                    response.AddOkResult(Resources.Common.CreateSuccessMessage);

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