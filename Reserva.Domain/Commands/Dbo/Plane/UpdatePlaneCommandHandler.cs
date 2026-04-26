using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Plane;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Plane
{
    public class UpdatePlaneCommandHandler : CommandHandlerBase<UpdatePlaneCommand, GetPlaneDto>
    {
        private readonly IRepository<Entity.Plane> _PlaneRepository;

        public UpdatePlaneCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UpdatePlaneCommandValidator validator,
            IRepository<Entity.Plane> PlaneRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _PlaneRepository = PlaneRepository;
        }

        public override async Task<ResponseDto<GetPlaneDto>> HandleCommand(UpdatePlaneCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetPlaneDto>();
            var Plane = await _PlaneRepository.GetByAsync(x => x.IdPlane == request.UpdateDto.IdPlane);

            if (Plane != null)
            {
                _mapper?.Map(request.UpdateDto, Plane);
                await _PlaneRepository.UpdateAsync(Plane);
                await _PlaneRepository.SaveAsync();
            }

            var PlaneDto = _mapper?.Map<GetPlaneDto>(Plane);
            if (PlaneDto != null) response.UpdateData(PlaneDto);

            response.AddOkResult(Resources.Common.UpdateSuccessMessage);

            return await Task.FromResult(response);
        }
    }
}
