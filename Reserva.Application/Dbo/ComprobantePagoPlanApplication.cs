using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Dbo.ComprobantePagoPlan;
using Reserva.Domain.Queries.Dbo.ComprobantePagoPlan;
using Reserva.Dto.Dbo.ComprobantePagoPlan;

namespace Reserva.Application.Dbo
{
    public class ComprobantePagoPlanApplication : ApplicationBase, IComprobantePagoPlanApplication
    {
        public ComprobantePagoPlanApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetComprobantePagoPlanDto>> Create(CreateComprobantePagoPlanDto createDto)
            => await _mediator.Send(new CreateComprobantePagoPlanCommand(createDto));
        public async Task<ResponseDto<GetComprobantePagoPlanDto>> Update(UpdateComprobantePagoPlanDto updateDto)
            => await _mediator.Send(new UpdateComprobantePagoPlanCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteComprobantePagoPlanCommand(id));
        public async Task<ResponseDto<GetComprobantePagoPlanDto>> Get(int id)
            => await _mediator.Send(new GetComprobantePagoPlanQuery(id));
        public async Task<ResponseDto<IEnumerable<ListComprobantePagoPlanDto>>> List(int id)
            => await _mediator.Send(new ListComprobantePagoPlanQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchComprobantePagoPlanDto>>> Search(SearchParamsDto<SearchComprobantePagoPlanFilterDto> searchParams)
            => await _mediator.Send(new SearchComprobantePagoPlanQuery(searchParams));

    }
}
