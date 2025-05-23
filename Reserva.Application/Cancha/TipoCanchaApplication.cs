using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Cancha.TipoCancha;
using Reserva.Domain.Queries.Cancha.TipoCancha;
using Reserva.Dto.Cancha.TipoCancha;

namespace Reserva.Application.Cancha
{
    public class TipoCanchaApplication : ApplicationBase, ITipoCanchaApplication
    {
        public TipoCanchaApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetTipoCanchaDto>> Create(CreateTipoCanchaDto createDto)
            => await _mediator.Send(new CreateTipoCanchaCommand(createDto));
        public async Task<ResponseDto<GetTipoCanchaDto>> Update(UpdateTipoCanchaDto updateDto)
            => await _mediator.Send(new UpdateTipoCanchaCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteTipoCanchaCommand(id));
        public async Task<ResponseDto<GetTipoCanchaDto>> Get(int id)
            => await _mediator.Send(new GetTipoCanchaQuery(id));
        public async Task<ResponseDto<IEnumerable<ListTipoCanchaDto>>> List(int id)
            => await _mediator.Send(new ListTipoCanchaQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchTipoCanchaDto>>> Search(SearchParamsDto<SearchTipoCanchaFilterDto> searchParams)
            => await _mediator.Send(new SearchTipoCanchaQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboTipoCanchaDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboTipoCanchaQuery());
        public async Task<ResponseDto<SearchResultDto<SelectTipoCanchaDto>>> Select(SearchParamsDto<SelectTipoCanchaFilterDto> searchParams)
             => await _mediator.Send(new SelectTipoCanchaQuery(searchParams));

    }
}
