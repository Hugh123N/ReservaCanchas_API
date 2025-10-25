using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Dbo.EstadoCancha;
using Reserva.Domain.Queries.Dbo.EstadoCancha;
using Reserva.Dto.Dbo.EstadoCancha;

namespace Reserva.Application.Dbo
{
    public class EstadoCanchaApplication : ApplicationBase, IEstadoCanchaApplication
    {
        public EstadoCanchaApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetEstadoCanchaDto>> Create(CreateEstadoCanchaDto createDto)
            => await _mediator.Send(new CreateEstadoCanchaCommand(createDto));
        public async Task<ResponseDto<GetEstadoCanchaDto>> Update(UpdateEstadoCanchaDto updateDto)
            => await _mediator.Send(new UpdateEstadoCanchaCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteEstadoCanchaCommand(id));
        public async Task<ResponseDto<GetEstadoCanchaDto>> Get(int id)
            => await _mediator.Send(new GetEstadoCanchaQuery(id));
        public async Task<ResponseDto<IEnumerable<ListEstadoCanchaDto>>> List(int id)
            => await _mediator.Send(new ListEstadoCanchaQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchEstadoCanchaDto>>> Search(SearchParamsDto<SearchEstadoCanchaFilterDto> searchParams)
            => await _mediator.Send(new SearchEstadoCanchaQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboEstadoCanchaDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboEstadoCanchaQuery());
        public async Task<ResponseDto<SearchResultDto<SelectEstadoCanchaDto>>> Select(SearchParamsDto<SelectEstadoCanchaFilterDto> searchParams)
             => await _mediator.Send(new SelectEstadoCanchaQuery(searchParams));

    }
}
