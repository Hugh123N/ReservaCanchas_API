using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reserva.Dto.Base;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.PagoPlan;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.PagoPlan
{
    public class GetPaymentsProveedorPlanQueryHandler : QueryHandlerBase<GetPaymentsProveedorPlanQuery, List<GetPagoPlanDto>>
    {
        private readonly IRepository<Entity.ProveedorPlan> _proveedorPlanRepository;
        private readonly IRepository<Entity.EstadoPago> _estadoPagoRepository;

        public GetPaymentsProveedorPlanQueryHandler(
            IMapper mapper,
            IRepository<Entity.ProveedorPlan> proveedorPlanRepository,
            IRepository<Entity.EstadoPago> estadoPagoRepository
        ) : base(mapper)
        {
            _proveedorPlanRepository = proveedorPlanRepository;
            _estadoPagoRepository = estadoPagoRepository;
        }

        protected override async Task<ResponseDto<List<GetPagoPlanDto>>> HandleQuery(GetPaymentsProveedorPlanQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<List<GetPagoPlanDto>>();

            var subscriptions = await _proveedorPlanRepository.FindByAsync(
                x => x.IdProveedor == request.IdProveedor && x.Activo,
                x => x.PagoPlan.Where(p => p.Activo)
            );
            var estadoPagos = await _estadoPagoRepository.FindByAsNoTrackingAsync(x => x.Activo);

            var allPagos = _mapper!.Map<List<GetPagoPlanDto>>(subscriptions.SelectMany(s => s.PagoPlan).ToList());

            allPagos.ForEach(p =>
            {
                var estadoPago = estadoPagos.FirstOrDefault(e => e.IdEstadoPago == p.IdEstadoPago);
                if (estadoPago != null)
                {
                    p.EstadoPago = estadoPago.Nombre;
                }
            });

            var pagosOrdenados = allPagos
                .OrderByDescending(p => p.FechaPago)
                .ToList();

            response.UpdateData(pagosOrdenados);
            return response;
        }
    }
}