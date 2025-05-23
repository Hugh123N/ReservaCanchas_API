using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Provincia;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Provincia
{
    public class UpdateProvinciaCommandHandler : CommandHandlerBase<UpdateProvinciaCommand, GetProvinciaDto>
    {
        private readonly IRepository<Entity.Models.Provincia> _ProvinciaRepository;

        public UpdateProvinciaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateProvinciaCommandValidator validator,
            IRepository<Entity.Models.Provincia> ProvinciaRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _ProvinciaRepository = ProvinciaRepository;
        }

        public override async Task<ResponseDto<GetProvinciaDto>> HandleCommand(UpdateProvinciaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetProvinciaDto>();
            var Provincia = await _ProvinciaRepository.GetByAsync(x => x.IdProvincia == request.UpdateDto.IdProvincia);

            if (Provincia != null)
            {
                _mapper?.Map(request.UpdateDto, Provincia);
                await _ProvinciaRepository.UpdateAsync(Provincia);
                await _ProvinciaRepository.SaveAsync();
            }

            var ProvinciaDto = _mapper?.Map<GetProvinciaDto>(Provincia);
            if (ProvinciaDto != null) response.UpdateData(ProvinciaDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
