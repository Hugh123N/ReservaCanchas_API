using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Cancha.EstadoReserva;
using Reserva.Domain.Queries.Cancha.EstadoReserva;
using Reserva.Dto.Cancha.EstadoReserva;

namespace Reserva.Application.Cancha
{
    public class EstadoReservaApplication : ApplicationBase, IEstadoReservaApplication
    {
        public EstadoReservaApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetEstadoReservaDto>> Create(CreateEstadoReservaDto createDto)
            => await _mediator.Send(new CreateEstadoReservaCommand(createDto));
        public async Task<ResponseDto<GetEstadoReservaDto>> Update(UpdateEstadoReservaDto updateDto)
            => await _mediator.Send(new UpdateEstadoReservaCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteEstadoReservaCommand(id));
        public async Task<ResponseDto<GetEstadoReservaDto>> Get(int id)
            => await _mediator.Send(new GetEstadoReservaQuery(id));
        public async Task<ResponseDto<IEnumerable<ListEstadoReservaDto>>> List(int id)
            => await _mediator.Send(new ListEstadoReservaQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchEstadoReservaDto>>> Search(SearchParamsDto<SearchEstadoReservaFilterDto> searchParams)
            => await _mediator.Send(new SearchEstadoReservaQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboEstadoReservaDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboEstadoReservaQuery());
        public async Task<ResponseDto<SearchResultDto<SelectEstadoReservaDto>>> Select(SearchParamsDto<SelectEstadoReservaFilterDto> searchParams)
             => await _mediator.Send(new SelectEstadoReservaQuery(searchParams));

    }
}
