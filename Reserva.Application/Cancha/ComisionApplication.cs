using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Cancha.Comision;
using Reserva.Domain.Queries.Cancha.Comision;
using Reserva.Dto.Cancha.Comision;

namespace Reserva.Application.Cancha
{
    public class ComisionApplication : ApplicationBase, IComisionApplication
    {
        public ComisionApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetComisionDto>> Create(CreateComisionDto createDto)
            => await _mediator.Send(new CreateComisionCommand(createDto));
        public async Task<ResponseDto<GetComisionDto>> Update(UpdateComisionDto updateDto)
            => await _mediator.Send(new UpdateComisionCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteComisionCommand(id));
        public async Task<ResponseDto<GetComisionDto>> Get(int id)
            => await _mediator.Send(new GetComisionQuery(id));
        public async Task<ResponseDto<IEnumerable<ListComisionDto>>> List(int id)
            => await _mediator.Send(new ListComisionQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchComisionDto>>> Search(SearchParamsDto<SearchComisionFilterDto> searchParams)
            => await _mediator.Send(new SearchComisionQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboComisionDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboComisionQuery());
        public async Task<ResponseDto<SearchResultDto<SelectComisionDto>>> Select(SearchParamsDto<SelectComisionFilterDto> searchParams)
             => await _mediator.Send(new SelectComisionQuery(searchParams));

    }
}
