using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Dbo.ProveedorPlan;
using Reserva.Domain.Queries.Dbo.ProveedorPlan;
using Reserva.Dto.Dbo.ProveedorPlan;

namespace Reserva.Application.Dbo
{
    public class ProveedorPlanApplication : ApplicationBase, IProveedorPlanApplication
    {
        public ProveedorPlanApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetProveedorPlanDto>> Create(CreateProveedorPlanDto createDto)
            => await _mediator.Send(new CreateProveedorPlanCommand(createDto));
        public async Task<ResponseDto<GetProveedorPlanDto>> Update(UpdateProveedorPlanDto updateDto)
            => await _mediator.Send(new UpdateProveedorPlanCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteProveedorPlanCommand(id));
        public async Task<ResponseDto<GetProveedorPlanDto>> Get(int id)
            => await _mediator.Send(new GetProveedorPlanQuery(id));
        public async Task<ResponseDto<IEnumerable<ListProveedorPlanDto>>> List(int id)
            => await _mediator.Send(new ListProveedorPlanQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchProveedorPlanDto>>> Search(SearchParamsDto<SearchProveedorPlanFilterDto> searchParams)
            => await _mediator.Send(new SearchProveedorPlanQuery(searchParams));

        public async Task<ResponseDto<GetProveedorPlanCurrentDto>> GetCurrent(int idProveedor)
            => await _mediator.Send(new GetCurrentProveedorPlanQuery(idProveedor));

        public async Task<ResponseDto<CheckoutResponseDto>> Checkout(CheckoutPlanDto checkoutDto)
            => await _mediator.Send(new CheckoutPlanCommand(checkoutDto));

        public async Task<ResponseDto<List<PagoPlanDto>>> GetPayments(int idProveedor)
            => await _mediator.Send(new GetPaymentsProveedorPlanQuery(idProveedor));

        public async Task<ResponseDto> CancelAutoRenew(int idProveedorPlan)
            => await _mediator.Send(new CancelAutoRenewCommand(idProveedorPlan));

        public async Task<ResponseDto> RetryPayment(RetryPaymentDto retryPaymentDto)
            => await _mediator.Send(new RetryPaymentPlanCommand(retryPaymentDto));

    }
}
