using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Cancha.Usuario;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Usuario
{
    public class CreateUsuarioCommandHandler : CommandHandlerBase<CreateUsuarioCommand, GetUsuarioDto>
    {
        private readonly IRepository<Entity.Models.Usuario> _UsuarioRepository;
        private readonly UserManager<Entity.Models.Usuario> _UsuarioManager;
        private readonly IRepository<Entity.Models.Rol> _RolRepository;
        private readonly IConfiguration _configuration;


        public CreateUsuarioCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateUsuarioCommandValidator validator,
            IRepository<Entity.Models.Usuario> UsuarioRepository,
            UserManager<Entity.Models.Usuario> userManager,
            IRepository<Entity.Models.Rol> RolRepository,
            IConfiguration configuration
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _UsuarioRepository = UsuarioRepository;
            _UsuarioManager = userManager;
            _configuration = configuration;
            _RolRepository = RolRepository;
        }

        public override async Task<ResponseDto<GetUsuarioDto>> HandleCommand(CreateUsuarioCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetUsuarioDto>();

            var Usuario = _mapper?.Map<Entity.Models.Usuario>(request.CreateDto);

            if (Usuario != null)
            {
                _UsuarioRepository.UpdateAuditTrails(Usuario);
                var result = await _UsuarioManager.CreateAsync(Usuario, request.CreateDto.Password);

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

            var UsuarioDto = _mapper?.Map<GetUsuarioDto>(Usuario);
            if (UsuarioDto != null) response.UpdateData(UsuarioDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}