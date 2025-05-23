using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.DiaSemana;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.DiaSemana
{
    public class UpdateDiaSemanaCommandHandler : CommandHandlerBase<UpdateDiaSemanaCommand, GetDiaSemanaDto>
    {
        private readonly IRepository<Entity.Models.DiaSemana> _DiaSemanaRepository;

        public UpdateDiaSemanaCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateDiaSemanaCommandValidator validator,
            IRepository<Entity.Models.DiaSemana> DiaSemanaRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _DiaSemanaRepository = DiaSemanaRepository;
        }

        public override async Task<ResponseDto<GetDiaSemanaDto>> HandleCommand(UpdateDiaSemanaCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetDiaSemanaDto>();
            var DiaSemana = await _DiaSemanaRepository.GetByAsync(x => x.IdDiaSemana == request.UpdateDto.IdDiaSemana);

            if (DiaSemana != null)
            {
                _mapper?.Map(request.UpdateDto, DiaSemana);
                await _DiaSemanaRepository.UpdateAsync(DiaSemana);
                await _DiaSemanaRepository.SaveAsync();
            }

            var DiaSemanaDto = _mapper?.Map<GetDiaSemanaDto>(DiaSemana);
            if (DiaSemanaDto != null) response.UpdateData(DiaSemanaDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
