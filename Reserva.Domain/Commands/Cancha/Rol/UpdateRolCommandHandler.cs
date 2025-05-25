using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Rol;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Rol
{
    public class UpdateRolCommandHandler : CommandHandlerBase<UpdateRolCommand, GetRolDto>
    {
        private readonly IRepository<Entity.Models.Rol> _RolRepository;

        public UpdateRolCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateRolCommandValidator validator,
            IRepository<Entity.Models.Rol> RolRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _RolRepository = RolRepository;
        }

        public override async Task<ResponseDto<GetRolDto>> HandleCommand(UpdateRolCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetRolDto>();
            var Rol = await _RolRepository.GetByAsync(x => x.IdRol == request.UpdateDto.IdRol);

            if (Rol != null)
            {
                _mapper?.Map(request.UpdateDto, Rol);
                await _RolRepository.UpdateAsync(Rol);
                await _RolRepository.SaveAsync();
            }

            var RolDto = _mapper?.Map<GetRolDto>(Rol);
            if (RolDto != null) response.UpdateData(RolDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
