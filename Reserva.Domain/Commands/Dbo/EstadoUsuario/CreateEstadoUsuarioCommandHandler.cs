using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.EstadoUsuario;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.EstadoUsuario
{
    public class CreateEstadoUsuarioCommandHandler : CommandHandlerBase<CreateEstadoUsuarioCommand, GetEstadoUsuarioDto>
    {
        private readonly IRepository<Entity.EstadoUsuario> _EstadoUsuarioRepository;

        public CreateEstadoUsuarioCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateEstadoUsuarioCommandValidator validator,
            IRepository<Entity.EstadoUsuario> EstadoUsuarioRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _EstadoUsuarioRepository = EstadoUsuarioRepository;
        }

        public override async Task<ResponseDto<GetEstadoUsuarioDto>> HandleCommand(CreateEstadoUsuarioCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetEstadoUsuarioDto>();

            var EstadoUsuario = _mapper?.Map<Entity.EstadoUsuario>(request.CreateDto);

            if (EstadoUsuario != null)
            {
                await _EstadoUsuarioRepository.AddAsync(EstadoUsuario);
                await _EstadoUsuarioRepository.SaveAsync();
            }

            var EstadoUsuarioDto = _mapper?.Map<GetEstadoUsuarioDto>(EstadoUsuario);
            if (EstadoUsuarioDto != null) response.UpdateData(EstadoUsuarioDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}