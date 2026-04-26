using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Plane;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Plane
{
    public class CreatePlaneCommandHandler : CommandHandlerBase<CreatePlaneCommand, GetPlaneDto>
    {
        private readonly IRepository<Entity.Plane> _PlaneRepository;

        public CreatePlaneCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CreatePlaneCommandValidator validator,
            IRepository<Entity.Plane> PlaneRepository
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _PlaneRepository = PlaneRepository;
        }

        public override async Task<ResponseDto<GetPlaneDto>> HandleCommand(CreatePlaneCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetPlaneDto>();

            var Plane = _mapper?.Map<Entity.Plane>(request.CreateDto);

            if (Plane != null)
            {
                await _PlaneRepository.AddAsync(Plane);
                await _PlaneRepository.SaveAsync();
            }

            var PlaneDto = _mapper?.Map<GetPlaneDto>(Plane);
            if (PlaneDto != null) response.UpdateData(PlaneDto);

            response.AddOkResult(Resources.Common.CreateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}