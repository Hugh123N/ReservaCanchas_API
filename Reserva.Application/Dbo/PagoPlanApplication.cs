using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Dbo.PagoPlan;
using Reserva.Domain.Queries.Dbo.PagoPlan;
using Reserva.Dto.Dbo.PagoPlan;

namespace Reserva.Application.Dbo
{
    public class PagoPlanApplication : ApplicationBase, IPagoPlanApplication
    {
        public PagoPlanApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetPagoPlanDto>> Create(CreatePagoPlanDto createDto)
            => await _mediator.Send(new CreatePagoPlanCommand(createDto));
        public async Task<ResponseDto<GetPagoPlanDto>> Update(UpdatePagoPlanDto updateDto)
            => await _mediator.Send(new UpdatePagoPlanCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeletePagoPlanCommand(id));
        public async Task<ResponseDto<GetPagoPlanDto>> Get(int id)
            => await _mediator.Send(new GetPagoPlanQuery(id));
        public async Task<ResponseDto<IEnumerable<ListPagoPlanDto>>> List(int id)
            => await _mediator.Send(new ListPagoPlanQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchPagoPlanDto>>> Search(SearchParamsDto<SearchPagoPlanFilterDto> searchParams)
            => await _mediator.Send(new SearchPagoPlanQuery(searchParams));

        public async Task<ResponseDto<List<GetPagoPlanDto>>> GetPayments(int idProveedor)
            => await _mediator.Send(new GetPaymentsProveedorPlanQuery(idProveedor));
    }
}
