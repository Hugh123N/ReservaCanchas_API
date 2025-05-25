using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Cancha.Pago;
using Reserva.Domain.Queries.Cancha.Pago;
using Reserva.Dto.Cancha.Pago;

namespace Reserva.Application.Cancha
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

    }
}
