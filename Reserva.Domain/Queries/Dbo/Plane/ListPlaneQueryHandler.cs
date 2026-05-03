using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Plane;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Plane
{
    public class ListPlaneQueryHandler : QueryHandlerBase<ListPlaneQuery, IEnumerable<ListPlaneDto>>
    {
        private readonly IRepository<Entity.Plane> _repository;

        public ListPlaneQueryHandler(
            IMapper mapper,
            IRepository<Entity.Plane> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListPlaneDto>>> HandleQuery(ListPlaneQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListPlaneDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.Activo, 
                x => x.PlanCaracteristica, 
                x => x.PlanTarifa
            );
            
            var listDtos = _mapper?.Map<List<ListPlaneDto>>(list);

            response.UpdateData(listDtos ?? new List<ListPlaneDto>());

            return await Task.FromResult(response);
        }
    }
}
