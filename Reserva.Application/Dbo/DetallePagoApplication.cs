using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Dbo.DetallePago;
using Reserva.Domain.Queries.Dbo.DetallePago;
using Reserva.Dto.Dbo.DetallePago;

namespace Reserva.Application.Dbo
{
    public class DetallePagoApplication : ApplicationBase, IDetallePagoApplication
    {
        public DetallePagoApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetDetallePagoDto>> Create(CreateDetallePagoDto createDto)
            => await _mediator.Send(new CreateDetallePagoCommand(createDto));
        public async Task<ResponseDto<GetDetallePagoDto>> Update(UpdateDetallePagoDto updateDto)
            => await _mediator.Send(new UpdateDetallePagoCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteDetallePagoCommand(id));
        public async Task<ResponseDto<GetDetallePagoDto>> Get(int id)
            => await _mediator.Send(new GetDetallePagoQuery(id));
        public async Task<ResponseDto<IEnumerable<ListDetallePagoDto>>> List(int id)
            => await _mediator.Send(new ListDetallePagoQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchDetallePagoDto>>> Search(SearchParamsDto<SearchDetallePagoFilterDto> searchParams)
            => await _mediator.Send(new SearchDetallePagoQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboDetallePagoDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboDetallePagoQuery());
        public async Task<ResponseDto<SearchResultDto<SelectDetallePagoDto>>> Select(SearchParamsDto<SelectDetallePagoFilterDto> searchParams)
             => await _mediator.Send(new SelectDetallePagoQuery(searchParams));

    }
}
