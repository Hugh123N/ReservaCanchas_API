using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Comision;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Comision
{
    public class UpdateComisionCommandHandler : CommandHandlerBase<UpdateComisionCommand, GetComisionDto>
    {
        private readonly IRepository<Entity.Models.Comision> _ComisionRepository;

        public UpdateComisionCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdateComisionCommandValidator validator,
            IRepository<Entity.Models.Comision> ComisionRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _ComisionRepository = ComisionRepository;
        }

        public override async Task<ResponseDto<GetComisionDto>> HandleCommand(UpdateComisionCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetComisionDto>();
            var Comision = await _ComisionRepository.GetByAsync(x => x.IdComision == request.UpdateDto.IdComision);

            if (Comision != null)
            {
                _mapper?.Map(request.UpdateDto, Comision);
                await _ComisionRepository.UpdateAsync(Comision);
                await _ComisionRepository.SaveAsync();
            }

            var ComisionDto = _mapper?.Map<GetComisionDto>(Comision);
            if (ComisionDto != null) response.UpdateData(ComisionDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
