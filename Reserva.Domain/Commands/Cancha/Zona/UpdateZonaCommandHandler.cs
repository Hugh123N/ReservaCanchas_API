using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Zona;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Zona
{
    public class UpdateZonaCommandHandler : CommandHandlerBase<UpdateZonaCommand, GetZonaDto>
    {
        private readonly IRepository<Entity.Models.Zona> _ZonaRepository;

        public UpdateZonaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateZonaCommandValidator validator,
            IRepository<Entity.Models.Zona> ZonaRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _ZonaRepository = ZonaRepository;
        }

        public override async Task<ResponseDto<GetZonaDto>> HandleCommand(UpdateZonaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetZonaDto>();
            var Zona = await _ZonaRepository.GetByAsync(x => x.IdZona == request.UpdateDto.IdZona);

            if (Zona != null)
            {
                _mapper?.Map(request.UpdateDto, Zona);
                await _ZonaRepository.UpdateAsync(Zona);
                await _ZonaRepository.SaveAsync();
            }

            var ZonaDto = _mapper?.Map<GetZonaDto>(Zona);
            if (ZonaDto != null) response.UpdateData(ZonaDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
