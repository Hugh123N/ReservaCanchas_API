using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Distrito;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Distrito
{
    public class UpdateDistritoCommandHandler : CommandHandlerBase<UpdateDistritoCommand, GetDistritoDto>
    {
        private readonly IRepository<Entity.Models.Distrito> _DistritoRepository;

        public UpdateDistritoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateDistritoCommandValidator validator,
            IRepository<Entity.Models.Distrito> DistritoRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _DistritoRepository = DistritoRepository;
        }

        public override async Task<ResponseDto<GetDistritoDto>> HandleCommand(UpdateDistritoCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetDistritoDto>();
            var Distrito = await _DistritoRepository.GetByAsync(x => x.IdDistrito == request.UpdateDto.IdDistrito);

            if (Distrito != null)
            {
                _mapper?.Map(request.UpdateDto, Distrito);
                await _DistritoRepository.UpdateAsync(Distrito);
                await _DistritoRepository.SaveAsync();
            }

            var DistritoDto = _mapper?.Map<GetDistritoDto>(Distrito);
            if (DistritoDto != null) response.UpdateData(DistritoDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
