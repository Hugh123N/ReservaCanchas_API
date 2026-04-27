using AutoMapper;
using FluentValidation;
using MediatR;
using Reserva.Common;
using Reserva.Domain.Commands.Base;
using Reserva.Domain.Services.Culqi;
using Reserva.Dto.Dbo.ProveedorPlan;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Abstractions.Transactions;
using Reserva.Entity;
using Microsoft.EntityFrameworkCore;

namespace Reserva.Domain.Commands.Dbo.ProveedorPlan
{
    public class CheckoutPlanCommandHandler : CommandHandlerBase<CheckoutPlanCommand, CheckoutResponseDto>
    {
        private readonly IRepository<Entity.ProveedorPlan> _proveedorPlanRepository;
        private readonly IRepository<Entity.Plane> _planeRepository;
        private readonly IRepository<Entity.PlanTarifa> _tarifaRepository;
        private readonly IRepository<Entity.EstadoPago> _estadoPagoRepository;
        private readonly IRepository<Entity.MetodoPago> _metodoPagoRepository;
        private readonly IRepository<Entity.PagoPlan> _pagoPlanRepository;
        private readonly CulqiService _culqiService;

        public CheckoutPlanCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator,
            CheckoutPlanCommandValidator validator,
            IRepository<Entity.ProveedorPlan> proveedorPlanRepository,
            IRepository<Entity.Plane> planeRepository,
            IRepository<Entity.PlanTarifa> tarifaRepository,
            IRepository<Entity.EstadoPago> estadoPagoRepository,
            IRepository<Entity.MetodoPago> metodoPagoRepository,
            IRepository<Entity.PagoPlan> pagoPlanRepository,
            CulqiService culqiService
        ) : base(unitOfWork, mapper, mediator, validator)
        {
            _proveedorPlanRepository = proveedorPlanRepository;
            _planeRepository = planeRepository;
            _tarifaRepository = tarifaRepository;
            _estadoPagoRepository = estadoPagoRepository;
            _metodoPagoRepository = metodoPagoRepository;
            _pagoPlanRepository = pagoPlanRepository;
            _culqiService = culqiService;
        }

        public override async Task<ResponseDto<CheckoutResponseDto>> HandleCommand(CheckoutPlanCommand request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<CheckoutResponseDto>();
            var dto = request.CheckoutDto;

            var tarifa = await _tarifaRepository.GetByAsync(x => x.IdPlanTarifa == dto.IdPlanTarifa, x => x.IdPlaneNavigation);
            if (tarifa == null)
            {
                response.AddErrorResult("Tarifa no encontrada");
                return response;
            }

            var estadoPendiente = await _estadoPagoRepository.GetByAsNoTrackingAsync(x => x.Codigo == Constants.ESTADO_PAGO.Pendiente);
            var metodoPago = await _metodoPagoRepository.GetByAsNoTrackingAsync(x => x.Codigo == Constants.METODO_PAGO.Yape);

            decimal monto = tarifa.Precio;
            if (tarifa.PorcentajeDescuento.HasValue && tarifa.PorcentajeDescuento > 0)
            {
                monto = monto - (monto * tarifa.PorcentajeDescuento.Value / 100);
            }

            var culqiRequest = new CulqiCreateChargeRequest
            {
                Amount = CulqiService.ConvertToCents(monto),
                CurrencyCode = "PEN",
                Email = dto.Email,
                SourceId = dto.CulqiToken ?? "",
                Description = $"Plan {tarifa.IdPlaneNavigation?.Nombre} - {tarifa.Nombre}",
                Metadata = new Dictionary<string, string>
                {
                    { "plan_id", dto.IdPlane.ToString() },
                    { "proveedor_id", dto.IdProveedor.ToString() },
                    { "tarifa_id", dto.IdPlanTarifa.ToString() },
                    { "tipo", "plan_proveedor" }
                }
            };

            CulqiChargeResponse? culqiResponse = null;
            try
            {
                culqiResponse = await _culqiService.CreateChargeAsync(culqiRequest);
            }
            catch (CulqiException ex)
            {
                response.AddErrorResult(ex.UserMessage ?? "Error al procesar el pago con Culqi");
                return response;
            }

            var proveedorPlan = new Entity.ProveedorPlan
            {
                IdProveedor = dto.IdProveedor,
                IdPlane = dto.IdPlane,
                IdPlanTarifa = dto.IdPlanTarifa,
                FechaInicio = DateTimeOffset.UtcNow,
                FechaFin = DateTimeOffset.UtcNow.AddDays(tarifa.DuracionDias),
                FechaProximoCobro = null,
                Estado = "PENDING",
                AutoRenovacion = tarifa.PermiteAutoRenovacion ?? false,
                EsActual = true,
                CulqiSubscriptionId = culqiResponse?.Id,
                CulqiCustomerId = null,
                GracePeriodHasta = null,
                UserNameCreate = "Sistema",
                CreateDate = DateTimeOffset.UtcNow,
                Activo = true
            };

            var pagosAnteriores = await _proveedorPlanRepository.FindByAsync(x => x.IdProveedor == dto.IdProveedor && x.EsActual && x.Activo);
            foreach (var pp in pagosAnteriores)
            {
                pp.EsActual = false;
                await _proveedorPlanRepository.UpdateAsync(pp);
            }

            await _proveedorPlanRepository.AddAsync(proveedorPlan);
            await _proveedorPlanRepository.SaveAsync();

            var pagoPlan = new Entity.PagoPlan
            {
                IdProveedorPlan = proveedorPlan.IdProveedorPlan,
                Monto = monto,
                Moneda = "PEN",
                IdMetodoPago = metodoPago?.IdMetodoPago ?? 1,
                IdEstadoPago = estadoPendiente?.IdEstadoPago ?? 1,
                CulqiChargeId = culqiResponse?.Id,
                CodigoOperacion = culqiResponse?.ReferenceCode,
                Activo = true
            };

            await _pagoPlanRepository.AddAsync(pagoPlan);
            await _pagoPlanRepository.SaveAsync();

            var checkoutResponse = new CheckoutResponseDto
            {
                IdProveedorPlan = proveedorPlan.IdProveedorPlan,
                CulqiChargeId = culqiResponse?.Id,
                ReferenceCode = culqiResponse?.ReferenceCode,
                Monto = monto,
                Estado = "PENDIENTE"
            };

            response.UpdateData(checkoutResponse);
            response.AddOkResult("Pago iniciado. Espera la confirmación del webhook de Culqi.");

            return response;
        }
    }

    public class CheckoutPlanCommandValidator : CommandValidatorBase<CheckoutPlanCommand>
    {
        public CheckoutPlanCommandValidator()
        {
            RequiredInformation(x => x.CheckoutDto).DependentRules(() =>
            {
                RuleFor(x => x.CheckoutDto.IdProveedor)
                    .GreaterThan(0)
                    .WithMessage("El proveedor es requerido");

                RuleFor(x => x.CheckoutDto.IdPlane)
                    .GreaterThan(0)
                    .WithMessage("El plan es requerido");

                RuleFor(x => x.CheckoutDto.IdPlanTarifa)
                    .GreaterThan(0)
                    .WithMessage("La tarifa es requerida");

                RuleFor(x => x.CheckoutDto.CulqiToken)
                    .NotEmpty()
                    .WithMessage("El token de Culqi es requerido");

                RuleFor(x => x.CheckoutDto.Email)
                    .NotEmpty()
                    .EmailAddress()
                    .WithMessage("El email es requerido");
            });
        }
    }
}