using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Commands.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;

namespace Reserva.Domain.Commands.Dbo.Plane
{
    public class DeletePlaneCommandHandler : CommandHandlerBase<DeletePlaneCommand>
    {
        private readonly IRepository<Entity.Plane> _PlaneRepository;

        public DeletePlaneCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            DeletePlaneCommandValidator validator,
            IRepository<Entity.Plane> PlaneRepository
        ) : base(unitOfWork, mapper, validator)
        {
            _PlaneRepository = PlaneRepository;
        }

        public override async Task<ResponseDto> HandleCommand(DeletePlaneCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto();
            var Plane = await _PlaneRepository.GetByAsync(x => x.IdPlane == request.Id);

            if (Plane != null)
            {
                Plane.Activo = false;
                await _PlaneRepository.UpdateAsync(Plane);
                response.AddOkResult(Resources.Common.DeleteSuccessMessage);
            }

            return response;
        }
    }
}
