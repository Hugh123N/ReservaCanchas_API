using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.HorarioCancha;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.HorarioCancha
{
    public class UpdateHorarioCanchaCommandHandler : CommandHandlerBase<UpdateHorarioCanchaCommand, GetHorarioCanchaDto>
    {
        private readonly IRepository<Entity.HorarioCancha> _HorarioCanchaRepository;

        public UpdateHorarioCanchaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateHorarioCanchaCommandValidator validator,
            IRepository<Entity.HorarioCancha> HorarioCanchaRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _HorarioCanchaRepository = HorarioCanchaRepository;
        }

        public override async Task<ResponseDto<GetHorarioCanchaDto>> HandleCommand(UpdateHorarioCanchaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetHorarioCanchaDto>();
            var HorarioCancha = await _HorarioCanchaRepository.GetByAsync(x => x.IdHorarioCancha == request.UpdateDto.IdHorarioCancha);

            if (HorarioCancha != null)
            {
                _mapper?.Map(request.UpdateDto, HorarioCancha);
                await _HorarioCanchaRepository.UpdateAsync(HorarioCancha);
                await _HorarioCanchaRepository.SaveAsync();
            }

            var HorarioCanchaDto = _mapper?.Map<GetHorarioCanchaDto>(HorarioCancha);
            if (HorarioCanchaDto != null) response.UpdateData(HorarioCanchaDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
