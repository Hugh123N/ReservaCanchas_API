using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Servicio;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Servicio
{
    public class UpdateServicioCommandHandler : CommandHandlerBase<UpdateServicioCommand, GetServicioDto>
    {
        private readonly IRepository<Entity.Servicio> _ServicioRepository;

        public UpdateServicioCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateServicioCommandValidator validator,
            IRepository<Entity.Servicio> ServicioRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _ServicioRepository = ServicioRepository;
        }

        public override async Task<ResponseDto<GetServicioDto>> HandleCommand(UpdateServicioCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetServicioDto>();
            var Servicio = await _ServicioRepository.GetByAsync(x => x.IdServicio == request.UpdateDto.IdServicio);

            if (Servicio != null)
            {
                _mapper?.Map(request.UpdateDto, Servicio);
                await _ServicioRepository.UpdateAsync(Servicio);
                await _ServicioRepository.SaveAsync();
            }

            var ServicioDto = _mapper?.Map<GetServicioDto>(Servicio);
            if (ServicioDto != null) response.UpdateData(ServicioDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
