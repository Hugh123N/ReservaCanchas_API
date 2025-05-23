using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Cancha.Zona;
using Reserva.Domain.Queries.Cancha.Zona;
using Reserva.Dto.Cancha.Zona;

namespace Reserva.Application.Cancha
{
    public class ZonaApplication : ApplicationBase, IZonaApplication
    {
        public ZonaApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetZonaDto>> Create(CreateZonaDto createDto)
            => await _mediator.Send(new CreateZonaCommand(createDto));
        public async Task<ResponseDto<GetZonaDto>> Update(UpdateZonaDto updateDto)
            => await _mediator.Send(new UpdateZonaCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteZonaCommand(id));
        public async Task<ResponseDto<GetZonaDto>> Get(int id)
            => await _mediator.Send(new GetZonaQuery(id));
        public async Task<ResponseDto<IEnumerable<ListZonaDto>>> List(int id)
            => await _mediator.Send(new ListZonaQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchZonaDto>>> Search(SearchParamsDto<SearchZonaFilterDto> searchParams)
            => await _mediator.Send(new SearchZonaQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboZonaDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboZonaQuery());
        public async Task<ResponseDto<SearchResultDto<SelectZonaDto>>> Select(SearchParamsDto<SelectZonaFilterDto> searchParams)
             => await _mediator.Send(new SelectZonaQuery(searchParams));

    }
}
