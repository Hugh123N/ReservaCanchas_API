using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Dbo.Pago;
using Reserva.Domain.Queries.Dbo.Pago;
using Reserva.Dto.Dbo.Pago;

namespace Reserva.Application.Dbo
{
    public class PagoApplication : ApplicationBase, IPagoApplication
    {
        public PagoApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetPagoDto>> Create(CreatePagoDto createDto)
            => await _mediator.Send(new CreatePagoCommand(createDto));
        public async Task<ResponseDto<GetPagoDto>> Update(UpdatePagoDto updateDto)
            => await _mediator.Send(new UpdatePagoCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeletePagoCommand(id));
        public async Task<ResponseDto<GetPagoDto>> Get(int id)
            => await _mediator.Send(new GetPagoQuery(id));
        public async Task<ResponseDto<IEnumerable<ListPagoDto>>> List(int id)
            => await _mediator.Send(new ListPagoQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchPagoDto>>> Search(SearchParamsDto<SearchPagoFilterDto> searchParams)
            => await _mediator.Send(new SearchPagoQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboPagoDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboPagoQuery());
        public async Task<ResponseDto<SearchResultDto<SelectPagoDto>>> Select(SearchParamsDto<SelectPagoFilterDto> searchParams)
             => await _mediator.Send(new SelectPagoQuery(searchParams));

        public async Task<ResponseDto<GetPagoDto>> ConfirmarPago(ConfirmarPagoDto confirmarDto)
            => await _mediator.Send(new ConfirmarPagoCommand(confirmarDto));
    }
}
