using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Domain.Queries.Dbo.Hora;
using Reserva.Dto.Dbo.Hora;

namespace Reserva.Application.Dbo
{
    public class HoraApplication : ApplicationBase, IHoraApplication
    {
        public HoraApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetHoraDto>> Get(int id)
            => await _mediator.Send(new GetHoraQuery(id));
        public async Task<ResponseDto<IEnumerable<ListHoraDto>>> List(int id)
            => await _mediator.Send(new ListHoraQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchHoraDto>>> Search(SearchParamsDto<SearchHoraFilterDto> searchParams)
            => await _mediator.Send(new SearchHoraQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboHoraDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboHoraQuery());

    }
}
