using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.Plane;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Plane
{
    public class GetPlaneQueryHandler : QueryHandlerBase<GetPlaneQuery, GetPlaneDto>
    {
        private readonly IRepository<Entity.Plane> _PlaneRepository;

        public GetPlaneQueryHandler(
            IMapper mapper,
            GetPlaneQueryValidator validator,
            IRepository<Entity.Plane> PlaneRepository
        ) : base(mapper, validator)
        {
            _PlaneRepository = PlaneRepository;
        }

        protected override async Task<ResponseDto<GetPlaneDto>> HandleQuery(GetPlaneQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetPlaneDto>();
            var Plane = await _PlaneRepository.GetByAsync(x => x.IdPlane == request.Id);
            var PlaneDto = _mapper?.Map<GetPlaneDto>(Plane);

            if (Plane != null && PlaneDto != null)
            {
                response.UpdateData(PlaneDto);
            }

            return await Task.FromResult(response);
        }
    }
}
