using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Comision;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Cancha.Comision
{
    public class CreateComisionCommandHandler : CommandHandlerBase<CreateComisionCommand, GetComisionDto>
    {
        private readonly IRepository<Entity.Models.Comision> _ComisionRepository;

        public CreateComisionCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreateComisionCommandValidator validator,
            IRepository<Entity.Models.Comision> ComisionRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _ComisionRepository = ComisionRepository;
        }

        public override async Task<ResponseDto<GetComisionDto>> HandleCommand(CreateComisionCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetComisionDto>();

            var Comision = _mapper?.Map<Entity.Models.Comision>(request.CreateDto);

            if (Comision != null)
            {
                await _ComisionRepository.AddAsync(Comision);
                await _ComisionRepository.SaveAsync();
            }

            var ComisionDto = _mapper?.Map<GetComisionDto>(Comision);
            if (ComisionDto != null) response.UpdateData(ComisionDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}