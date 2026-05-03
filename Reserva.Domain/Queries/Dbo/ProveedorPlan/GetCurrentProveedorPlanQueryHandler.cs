using AutoMapper;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.ProveedorPlan;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.ProveedorPlan
{
    public class GetCurrentProveedorPlanQueryHandler : QueryHandlerBase<GetCurrentProveedorPlanQuery, GetProveedorPlanCurrentDto>
    {
        private readonly IRepository<Entity.ProveedorPlan> _proveedorPlanRepository;

        public GetCurrentProveedorPlanQueryHandler(
            IMapper mapper,
            GetCurrentProveedorPlanQueryValidator validator,
            IRepository<Entity.ProveedorPlan> proveedorPlanRepository
        ) : base(mapper, validator)
        {
            _proveedorPlanRepository = proveedorPlanRepository;
        }

        protected override async Task<ResponseDto<GetProveedorPlanCurrentDto>> HandleQuery(GetCurrentProveedorPlanQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetProveedorPlanCurrentDto>();

            var proveedorPlan = await _proveedorPlanRepository.GetByAsync(
                x => x.IdProveedor == request.IdProveedor && x.EsActual && x.Activo,
                x => x.IdPlaneNavigation,
                x => x.IdPlaneNavigation.PlanCaracteristica,
                x => x.IdPlaneNavigation.PlanLimite,
                x => x.IdPlanTarifaNavigation
            );

            if (proveedorPlan == null)
            {
                response.AddWarningResult("El proveedor no tiene un plan activo");
                return response;
            }

            var dto = _mapper!.Map<GetProveedorPlanCurrentDto>(proveedorPlan);

            response.UpdateData(dto);
            return response;
        }
    }
}