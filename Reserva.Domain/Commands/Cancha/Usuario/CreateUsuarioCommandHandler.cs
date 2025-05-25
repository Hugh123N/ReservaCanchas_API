using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Usuario;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Usuario
{
    public class CreateUsuarioCommandHandler : CommandHandlerBase<CreateUsuarioCommand, GetUsuarioDto>
    {
        private readonly IRepository<Entity.Models.Usuario> _UsuarioRepository;

        public CreateUsuarioCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateUsuarioCommandValidator validator,
            IRepository<Entity.Models.Usuario> UsuarioRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _UsuarioRepository = UsuarioRepository;
        }

        public override async Task<ResponseDto<GetUsuarioDto>> HandleCommand(CreateUsuarioCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetUsuarioDto>();

            var Usuario = _mapper?.Map<Entity.Models.Usuario>(request.CreateDto);

            if (Usuario != null)
            {
                await _UsuarioRepository.AddAsync(Usuario);
                await _UsuarioRepository.SaveAsync();
            }

            var UsuarioDto = _mapper?.Map<GetUsuarioDto>(Usuario);
            if (UsuarioDto != null) response.UpdateData(UsuarioDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}