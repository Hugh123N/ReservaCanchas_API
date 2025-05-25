using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.EstadoUsuario;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.EstadoUsuario
{
    public class UpdateEstadoUsuarioCommandHandler : CommandHandlerBase<UpdateEstadoUsuarioCommand, GetEstadoUsuarioDto>
    {
        private readonly IRepository<Entity.Models.EstadoUsuario> _EstadoUsuarioRepository;

        public UpdateEstadoUsuarioCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateEstadoUsuarioCommandValidator validator,
            IRepository<Entity.Models.EstadoUsuario> EstadoUsuarioRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _EstadoUsuarioRepository = EstadoUsuarioRepository;
        }

        public override async Task<ResponseDto<GetEstadoUsuarioDto>> HandleCommand(UpdateEstadoUsuarioCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetEstadoUsuarioDto>();
            var EstadoUsuario = await _EstadoUsuarioRepository.GetByAsync(x => x.IdEstadoUsuario == request.UpdateDto.IdEstadoUsuario);

            if (EstadoUsuario != null)
            {
                _mapper?.Map(request.UpdateDto, EstadoUsuario);
                await _EstadoUsuarioRepository.UpdateAsync(EstadoUsuario);
                await _EstadoUsuarioRepository.SaveAsync();
            }

            var EstadoUsuarioDto = _mapper?.Map<GetEstadoUsuarioDto>(EstadoUsuario);
            if (EstadoUsuarioDto != null) response.UpdateData(EstadoUsuarioDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
