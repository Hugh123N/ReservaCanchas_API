using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Cancha;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Cancha
{
    public class UpdateCanchaCommandHandler : CommandHandlerBase<UpdateCanchaCommand, GetCanchaDto>
    {
        private readonly IRepository<Entity.Models.Cancha> _CanchaRepository;

        public UpdateCanchaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateCanchaCommandValidator validator,
            IRepository<Entity.Models.Cancha> CanchaRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _CanchaRepository = CanchaRepository;
        }

        public override async Task<ResponseDto<GetCanchaDto>> HandleCommand(UpdateCanchaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetCanchaDto>();
            var Cancha = await _CanchaRepository.GetByAsync(x => x.IdCancha == request.UpdateDto.IdCancha);

            if (Cancha != null)
            {
                _mapper?.Map(request.UpdateDto, Cancha);
                await _CanchaRepository.UpdateAsync(Cancha);
                await _CanchaRepository.SaveAsync();
            }

            var CanchaDto = _mapper?.Map<GetCanchaDto>(Cancha);
            if (CanchaDto != null) response.UpdateData(CanchaDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
