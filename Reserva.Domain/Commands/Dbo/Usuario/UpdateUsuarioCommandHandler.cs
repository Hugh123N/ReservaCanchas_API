using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Usuario;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Usuario
{
    public class UpdateUsuarioCommandHandler : CommandHandlerBase<UpdateUsuarioCommand, GetUsuarioDto>
    {
        private readonly IRepository<Entity.AspNetUsers> _UsuarioRepository;

        public UpdateUsuarioCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateUsuarioCommandValidator validator,
            IRepository<Entity.AspNetUsers> UsuarioRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _UsuarioRepository = UsuarioRepository;
        }

        public override async Task<ResponseDto<GetUsuarioDto>> HandleCommand(UpdateUsuarioCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetUsuarioDto>();
            var Usuario = await _UsuarioRepository.GetByAsync(x => x.Id == request.UpdateDto.Id);

            if (Usuario != null)
            {
                _mapper?.Map(request.UpdateDto, Usuario);
                await _UsuarioRepository.UpdateAsync(Usuario);
                await _UsuarioRepository.SaveAsync();
            }

            var UsuarioDto = _mapper?.Map<GetUsuarioDto>(Usuario);
            if (UsuarioDto != null) response.UpdateData(UsuarioDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
