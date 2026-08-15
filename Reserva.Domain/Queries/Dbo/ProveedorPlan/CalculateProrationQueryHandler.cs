using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.ProveedorPlan;
using Reserva.Repository.Abstractions.Base;
using Reserva.Entity;
using Reserva.Common;

namespace Reserva.Domain.Queries.Dbo.ProveedorPlan
{
    public class CalculateProrationQueryHandler : QueryHandlerBase<CalculateProrationQuery, CalculateProrationResponseDto>
    {
        private readonly IRepository<Entity.ProveedorPlan> _proveedorPlanRepository;
        private readonly IRepository<Entity.PlanTarifa> _tarifaRepository;

        public CalculateProrationQueryHandler(
            IMapper mapper,
            IRepository<Entity.ProveedorPlan> proveedorPlanRepository,
            IRepository<Entity.PlanTarifa> tarifaRepository
        ) : base(mapper)
        {
            _proveedorPlanRepository = proveedorPlanRepository;
            _tarifaRepository = tarifaRepository;
        }

        protected override async Task<ResponseDto<CalculateProrationResponseDto>> HandleQuery(
            CalculateProrationQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<CalculateProrationResponseDto>();
            var dto = request.Dto;

            var proveedorPlan = await _proveedorPlanRepository.GetByAsync(
                x => x.IdProveedorPlan == dto.IdProveedorPlan,
                x => x.IdPlanTarifaNavigation,
                x => x.IdPlaneNavigation
            );

            if (proveedorPlan == null)
            {
                response.AddErrorResult("Suscripción no encontrada");
                return response;
            }

            if (!proveedorPlan.EsActual || proveedorPlan.Estado != Constants.ESTADO_PROV_PLAN.ACTIVE)
            {
                response.AddErrorResult("La suscripción no está activa");
                return response;
            }

            var nuevaTarifa = await _tarifaRepository.GetByAsync(
                x => x.IdPlanTarifa == dto.IdNuevaPlanTarifa,
                x => x.IdPlaneNavigation
            );

            if (nuevaTarifa == null)
            {
                response.AddErrorResult("Nueva tarifa no encontrada");
                return response;
            }

            var tarifaActual = proveedorPlan.IdPlanTarifaNavigation;

            var ahora = DateTimeOffset.UtcNow;
            var diasRestantes = Math.Max(0, (int)(proveedorPlan.FechaFin - ahora).TotalDays);
            var duracionActual = tarifaActual.DuracionDias;
            var duracionNueva = nuevaTarifa.DuracionDias;

            decimal precioActual = tarifaActual.Precio;
            if (tarifaActual.PorcentajeDescuento.HasValue && tarifaActual.PorcentajeDescuento > 0)
            {
                precioActual = precioActual - (precioActual * tarifaActual.PorcentajeDescuento.Value / 100);
            }

            decimal precioNuevo = nuevaTarifa.Precio;
            if (nuevaTarifa.PorcentajeDescuento.HasValue && nuevaTarifa.PorcentajeDescuento > 0)
            {
                precioNuevo = precioNuevo - (nuevaTarifa.PorcentajeDescuento.Value / 100);
            }

            decimal creditoPlanActual = duracionActual > 0
                ? Math.Round((precioActual / duracionActual) * diasRestantes, 2)
                : 0;

            decimal cargoPlanNuevo = duracionNueva > 0
                ? Math.Round((precioNuevo / duracionNueva) * diasRestantes, 2)
                : 0;

            decimal saldoAFavorActual = proveedorPlan.SaldoFavor;

            decimal montoProrrateo = cargoPlanNuevo - creditoPlanActual - saldoAFavorActual;
            decimal nuevoSaldoAFavor = 0;

            if (montoProrrateo < 0)
            {
                nuevoSaldoAFavor = Math.Abs(montoProrrateo);
                montoProrrateo = 0;
            }

            var result = new CalculateProrationResponseDto
            {
                EsUpgrade = cargoPlanNuevo > creditoPlanActual,
                MontoProrrateo = montoProrrateo,
                CreditoPlanActual = creditoPlanActual,
                CargoPlanNuevo = cargoPlanNuevo,
                DiasRestantes = diasRestantes,
                SaldoAFavor = nuevoSaldoAFavor,
                Moneda = Constants.CURRENCY.PEN,
                NombrePlanActual = tarifaActual.IdPlaneNavigation?.Nombre ?? string.Empty,
                NombrePlanNuevo = nuevaTarifa.IdPlaneNavigation?.Nombre ?? string.Empty,
                PrecioPlanActual = precioActual,
                PrecioPlanNuevo = precioNuevo,
                FechaFinActual = proveedorPlan.FechaFin,
                FechaProximoCobro = proveedorPlan.FechaProximoCobro
            };

            response.UpdateData(result);
            return response;
        }
    }
}
