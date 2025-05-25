using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Cancha.Reserva;
using Reserva.Domain.Queries.Cancha.Reserva;
using Reserva.Dto.Cancha.Reserva;

namespace Reserva.Application.Cancha
{
    public class ReservaApplication : ApplicationBase, IReservaApplication
    {
        public ReservaApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetReservaDto>> Create(CreateReservaDto createDto)
            => await _mediator.Send(new CreateReservaCommand(createDto));
        public async Task<ResponseDto<GetReservaDto>> Update(UpdateReservaDto updateDto)
            => await _mediator.Send(new UpdateReservaCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteReservaCommand(id));
        public async Task<ResponseDto<GetReservaDto>> Get(int id)
            => await _mediator.Send(new GetReservaQuery(id));
        public async Task<ResponseDto<IEnumerable<ListReservaDto>>> List(int id)
            => await _mediator.Send(new ListReservaQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchReservaDto>>> Search(SearchParamsDto<SearchReservaFilterDto> searchParams)
            => await _mediator.Send(new SearchReservaQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboReservaDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboReservaQuery());
        public async Task<ResponseDto<SearchResultDto<SelectReservaDto>>> Select(SearchParamsDto<SelectReservaFilterDto> searchParams)
             => await _mediator.Send(new SelectReservaQuery(searchParams));

    }
}
