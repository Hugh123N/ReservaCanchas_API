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
                x => x.IdPlanTarifaNavigation
            );

            if (proveedorPlan == null)
            {
                response.AddWarningResult("El proveedor no tiene un plan activo");
                return response;
            }

            var dto = new GetProveedorPlanCurrentDto
            {
                IdProveedorPlan = proveedorPlan.IdProveedorPlan,
                IdProveedor = proveedorPlan.IdProveedor,
                IdPlane = proveedorPlan.IdPlane,
                IdPlanTarifa = proveedorPlan.IdPlanTarifa,
                FechaInicio = proveedorPlan.FechaInicio,
                FechaFin = proveedorPlan.FechaFin,
                FechaProximoCobro = proveedorPlan.FechaProximoCobro,
                Estado = proveedorPlan.Estado,
                AutoRenovacion = proveedorPlan.AutoRenovacion,
                EsActual = proveedorPlan.EsActual,
                CulqiSubscriptionId = proveedorPlan.CulqiSubscriptionId,
                CulqiCustomerId = proveedorPlan.CulqiCustomerId,
                GracePeriodHasta = proveedorPlan.GracePeriodHasta,
                FechaCancelacion = proveedorPlan.FechaCancelacion,
                MotivoCancelacion = proveedorPlan.MotivoCancelacion,
                Activo = proveedorPlan.Activo,
                NombrePlan = proveedorPlan.IdPlaneNavigation?.Nombre,
                DescripcionPlan = proveedorPlan.IdPlaneNavigation?.Descripcion,
                NombreTarifa = proveedorPlan.IdPlanTarifaNavigation?.Nombre,
                DuracionDias = proveedorPlan.IdPlanTarifaNavigation?.DuracionDias,
                TipoCobro = proveedorPlan.IdPlanTarifaNavigation?.TipoCobro,
                PrecioPlan = proveedorPlan.IdPlanTarifaNavigation?.Precio
            };

            response.UpdateData(dto);
            return response;
        }
    }
}