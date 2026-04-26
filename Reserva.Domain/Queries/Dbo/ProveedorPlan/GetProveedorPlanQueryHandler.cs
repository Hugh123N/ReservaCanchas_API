using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.ProveedorPlan;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.ProveedorPlan
{
    public class GetProveedorPlanQueryHandler : QueryHandlerBase<GetProveedorPlanQuery, GetProveedorPlanDto>
    {
        private readonly IRepository<Entity.ProveedorPlan> _ProveedorPlanRepository;

        public GetProveedorPlanQueryHandler(
            IMapper mapper,
            GetProveedorPlanQueryValidator validator,
            IRepository<Entity.ProveedorPlan> ProveedorPlanRepository
        ) : base(mapper, validator)
        {
            _ProveedorPlanRepository = ProveedorPlanRepository;
        }

        protected override async Task<ResponseDto<GetProveedorPlanDto>> HandleQuery(GetProveedorPlanQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetProveedorPlanDto>();
            var ProveedorPlan = await _ProveedorPlanRepository.GetByAsync(x => x.IdProveedorPlan == request.Id);
            var ProveedorPlanDto = _mapper?.Map<GetProveedorPlanDto>(ProveedorPlan);

            if (ProveedorPlan != null && ProveedorPlanDto != null)
            {
                response.UpdateData(ProveedorPlanDto);
            }

            return await Task.FromResult(response);
        }
    }
}
