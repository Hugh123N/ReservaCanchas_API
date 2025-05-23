using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Cancha.MetodoPago;
using Reserva.Domain.Queries.Cancha.MetodoPago;
using Reserva.Dto.Cancha.MetodoPago;

namespace Reserva.Application.Cancha
{
    public class MetodoPagoApplication : ApplicationBase, IMetodoPagoApplication
    {
        public MetodoPagoApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetMetodoPagoDto>> Create(CreateMetodoPagoDto createDto)
            => await _mediator.Send(new CreateMetodoPagoCommand(createDto));
        public async Task<ResponseDto<GetMetodoPagoDto>> Update(UpdateMetodoPagoDto updateDto)
            => await _mediator.Send(new UpdateMetodoPagoCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteMetodoPagoCommand(id));
        public async Task<ResponseDto<GetMetodoPagoDto>> Get(int id)
            => await _mediator.Send(new GetMetodoPagoQuery(id));
        public async Task<ResponseDto<IEnumerable<ListMetodoPagoDto>>> List(int id)
            => await _mediator.Send(new ListMetodoPagoQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchMetodoPagoDto>>> Search(SearchParamsDto<SearchMetodoPagoFilterDto> searchParams)
            => await _mediator.Send(new SearchMetodoPagoQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboMetodoPagoDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboMetodoPagoQuery());
        public async Task<ResponseDto<SearchResultDto<SelectMetodoPagoDto>>> Select(SearchParamsDto<SelectMetodoPagoFilterDto> searchParams)
             => await _mediator.Send(new SelectMetodoPagoQuery(searchParams));

    }
}
