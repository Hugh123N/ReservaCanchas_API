using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.ProveedorPlan;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.ProveedorPlan
{
    public class GetPaymentsProveedorPlanQueryHandler : QueryHandlerBase<GetPaymentsProveedorPlanQuery, List<PagoPlanDto>>
    {
        private readonly IRepository<Entity.ProveedorPlan> _proveedorPlanRepository;

        public GetPaymentsProveedorPlanQueryHandler(
            IMapper mapper,
            IRepository<Entity.ProveedorPlan> proveedorPlanRepository
        ) : base(mapper)
        {
            _proveedorPlanRepository = proveedorPlanRepository;
        }

        protected override async Task<ResponseDto<List<PagoPlanDto>>> HandleQuery(GetPaymentsProveedorPlanQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<List<PagoPlanDto>>();

            var subscriptions = await _proveedorPlanRepository.FindByAsync(
                x => x.IdProveedor == request.IdProveedor && x.Activo,
                x => x.PagoPlan
            );

            var allPagos = new List<PagoPlanDto>();

            foreach (var sub in subscriptions)
            {
                foreach (var pago in sub.PagoPlan ?? new List<Entity.PagoPlan>())
                {
                    allPagos.Add(new PagoPlanDto
                    {
                        IdPagoPlan = pago.IdPagoPlan,
                        IdProveedorPlan = pago.IdProveedorPlan,
                        Monto = pago.Monto,
                        Moneda = pago.Moneda,
                        IdMetodoPago = pago.IdMetodoPago,
                        IdEstadoPago = pago.IdEstadoPago,
                        FechaPago = pago.FechaPago,
                        CulqiChargeId = pago.CulqiChargeId,
                        CodigoOperacion = pago.CodigoOperacion,
                        Activo = pago.Activo
                    });
                }
            }

            var pagosOrdenados = allPagos
                .OrderByDescending(p => p.FechaPago)
                .ToList();

            response.UpdateData(pagosOrdenados);
            return response;
        }
    }
}