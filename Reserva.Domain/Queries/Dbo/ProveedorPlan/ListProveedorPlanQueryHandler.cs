using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.ProveedorPlan;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.ProveedorPlan
{
    public class ListProveedorPlanQueryHandler : QueryHandlerBase<ListProveedorPlanQuery, IEnumerable<ListProveedorPlanDto>>
    {
        private readonly IRepository<Entity.ProveedorPlan> _repository;

        public ListProveedorPlanQueryHandler(
            IMapper mapper,
            IRepository<Entity.ProveedorPlan> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListProveedorPlanDto>>> HandleQuery(ListProveedorPlanQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListProveedorPlanDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdProveedorPlan == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListProveedorPlanDto>>(list);

            response.UpdateData(listDtos ?? new List<ListProveedorPlanDto>());

            return await Task.FromResult(response);
        }
    }
}
