using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.PlanLimite;
using Reserva.Domain.Queries.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.PlanLimite
{
    public class ListPlanLimiteQueryHandler : QueryHandlerBase<ListPlanLimiteQuery, IEnumerable<ListPlanLimiteDto>>
    {
        private readonly IRepository<Entity.PlanLimite> _repository;

        public ListPlanLimiteQueryHandler(
            IMapper mapper,
            IRepository<Entity.PlanLimite> repository
        ) : base(mapper)
        {
            _repository = repository;
        }

        protected override async Task<ResponseDto<IEnumerable<ListPlanLimiteDto>>> HandleQuery(ListPlanLimiteQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<IEnumerable<ListPlanLimiteDto>>();
            var list = await _repository.FindByAsNoTrackingAsync(x => x.IdPlanLimite == request.Id);
            var listDtos = _mapper?.Map<IEnumerable<ListPlanLimiteDto>>(list);

            response.UpdateData(listDtos ?? new List<ListPlanLimiteDto>());

            return await Task.FromResult(response);
        }
    }
}
