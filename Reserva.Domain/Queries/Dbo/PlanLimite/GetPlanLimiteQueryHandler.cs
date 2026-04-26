using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.PlanLimite;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.PlanLimite
{
    public class GetPlanLimiteQueryHandler : QueryHandlerBase<GetPlanLimiteQuery, GetPlanLimiteDto>
    {
        private readonly IRepository<Entity.PlanLimite> _PlanLimiteRepository;

        public GetPlanLimiteQueryHandler(
            IMapper mapper,
            GetPlanLimiteQueryValidator validator,
            IRepository<Entity.PlanLimite> PlanLimiteRepository
        ) : base(mapper, validator)
        {
            _PlanLimiteRepository = PlanLimiteRepository;
        }

        protected override async Task<ResponseDto<GetPlanLimiteDto>> HandleQuery(GetPlanLimiteQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetPlanLimiteDto>();
            var PlanLimite = await _PlanLimiteRepository.GetByAsync(x => x.IdPlanLimite == request.Id);
            var PlanLimiteDto = _mapper?.Map<GetPlanLimiteDto>(PlanLimite);

            if (PlanLimite != null && PlanLimiteDto != null)
            {
                response.UpdateData(PlanLimiteDto);
            }

            return await Task.FromResult(response);
        }
    }
}
