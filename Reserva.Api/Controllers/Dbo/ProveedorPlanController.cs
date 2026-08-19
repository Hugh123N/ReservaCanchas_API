using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Dto.Dbo.Proveedor;
using Reserva.Dto.Dbo.ProveedorPlan;
using Reserva.Dto.User;

namespace Reserva.Api.Controllers.Dbo
{
    [ApiController]
    [Route("api/ProveedorPlan")]
    public class ProveedorPlanController : IProveedorPlanApplication
    {
        private readonly IProveedorPlanApplication _ProveedorPlanApplication;

        public ProveedorPlanController(IProveedorPlanApplication ProveedorPlanApplication)
            => _ProveedorPlanApplication = ProveedorPlanApplication;

        [HttpPost]
        public async Task<ResponseDto<GetProveedorPlanDto>> Create(CreateProveedorPlanDto createDto)
            => await _ProveedorPlanApplication.Create(createDto);
        [HttpPut]
        public async Task<ResponseDto<GetProveedorPlanDto>> Update(UpdateProveedorPlanDto updateDto)
            => await _ProveedorPlanApplication.Update(updateDto);
        [HttpDelete("{id}")]
        public async Task<ResponseDto> Delete(int id)
            => await _ProveedorPlanApplication.Delete(id);
        [HttpGet("{id}")]
        public async Task<ResponseDto<GetProveedorPlanDto>> Get(int id)
            => await _ProveedorPlanApplication.Get(id);
        [HttpPost("list")]
        public async Task<ResponseDto<IEnumerable<ListProveedorPlanDto>>> List(int id)
            => await _ProveedorPlanApplication.List(id);
        [HttpPost("search")]
        public async Task<ResponseDto<SearchResultDto<SearchProveedorPlanDto>>> Search(SearchParamsDto<SearchProveedorPlanFilterDto> searchParams)
            => await _ProveedorPlanApplication.Search(searchParams);

        [HttpGet("current/{idProveedor}")]
        public async Task<ResponseDto<GetProveedorPlanCurrentDto>> GetCurrent(int idProveedor)
            => await _ProveedorPlanApplication.GetCurrent(idProveedor);

        [HttpPost("checkout")]
        public async Task<ResponseDto> Checkout([FromBody] CheckoutPlanDto checkoutDto)
            => await _ProveedorPlanApplication.Checkout(checkoutDto);

        [HttpPost("cancel-auto-renew/{idProveedorPlan}")]
        public async Task<ResponseDto> CancelAutoRenew(int idProveedorPlan)
            => await _ProveedorPlanApplication.CancelAutoRenew(idProveedorPlan);

        [HttpPost("retry-payment")]
        public async Task<ResponseDto> RetryPayment([FromBody] RetryPaymentDto retryPaymentDto)
            => await _ProveedorPlanApplication.RetryPayment(retryPaymentDto);

        [HttpPost("change-plan")]
        public async Task<ResponseDto<ChangePlanResponseDto>> ChangePlan([FromBody] ChangePlanDto changePlanDto)
            => await _ProveedorPlanApplication.ChangePlan(changePlanDto);

        [HttpPost("calculate-proration")]
        public async Task<ResponseDto<CalculateProrationResponseDto>> CalculateProration([FromBody] CalculateProrationDto calculateProrationDto)
            => await _ProveedorPlanApplication.CalculateProration(calculateProrationDto);

        /// <summary>
        /// Registro de proveedor con plan gratuito (onboarding).
        /// Crea usuario + proveedor + plan + retorna token de acceso.
        /// </summary>
        [HttpPost("register-with-plan")]
        [AllowAnonymous]
        public async Task<ResponseDto<LoginResultDto>> RegisterWithPlan([FromBody] RegisterWithPlanDto registerWithPlanDto)
            => await _ProveedorPlanApplication.RegisterWithPlan(registerWithPlanDto);
    }
}
