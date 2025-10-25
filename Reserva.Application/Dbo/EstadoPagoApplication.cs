using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Dbo.EstadoPago;
using Reserva.Domain.Queries.Dbo.EstadoPago;
using Reserva.Dto.Dbo.EstadoPago;

namespace Reserva.Application.Dbo
{
    public class EstadoPagoApplication : ApplicationBase, IEstadoPagoApplication
    {
        public EstadoPagoApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetEstadoPagoDto>> Create(CreateEstadoPagoDto createDto)
            => await _mediator.Send(new CreateEstadoPagoCommand(createDto));
        public async Task<ResponseDto<GetEstadoPagoDto>> Update(UpdateEstadoPagoDto updateDto)
            => await _mediator.Send(new UpdateEstadoPagoCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteEstadoPagoCommand(id));
        public async Task<ResponseDto<GetEstadoPagoDto>> Get(int id)
            => await _mediator.Send(new GetEstadoPagoQuery(id));
        public async Task<ResponseDto<IEnumerable<ListEstadoPagoDto>>> List(int id)
            => await _mediator.Send(new ListEstadoPagoQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchEstadoPagoDto>>> Search(SearchParamsDto<SearchEstadoPagoFilterDto> searchParams)
            => await _mediator.Send(new SearchEstadoPagoQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboEstadoPagoDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboEstadoPagoQuery());
        public async Task<ResponseDto<SearchResultDto<SelectEstadoPagoDto>>> Select(SearchParamsDto<SelectEstadoPagoFilterDto> searchParams)
             => await _mediator.Send(new SelectEstadoPagoQuery(searchParams));

    }
}
