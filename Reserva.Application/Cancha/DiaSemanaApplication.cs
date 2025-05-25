using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Cancha.DiaSemana;
using Reserva.Domain.Queries.Cancha.DiaSemana;
using Reserva.Dto.Cancha.DiaSemana;

namespace Reserva.Application.Cancha
{
    public class DiaSemanaApplication : ApplicationBase, IDiaSemanaApplication
    {
        public DiaSemanaApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetDiaSemanaDto>> Create(CreateDiaSemanaDto createDto)
            => await _mediator.Send(new CreateDiaSemanaCommand(createDto));
        public async Task<ResponseDto<GetDiaSemanaDto>> Update(UpdateDiaSemanaDto updateDto)
            => await _mediator.Send(new UpdateDiaSemanaCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteDiaSemanaCommand(id));
        public async Task<ResponseDto<GetDiaSemanaDto>> Get(int id)
            => await _mediator.Send(new GetDiaSemanaQuery(id));
        public async Task<ResponseDto<IEnumerable<ListDiaSemanaDto>>> List(int id)
            => await _mediator.Send(new ListDiaSemanaQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchDiaSemanaDto>>> Search(SearchParamsDto<SearchDiaSemanaFilterDto> searchParams)
            => await _mediator.Send(new SearchDiaSemanaQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboDiaSemanaDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboDiaSemanaQuery());
        public async Task<ResponseDto<SearchResultDto<SelectDiaSemanaDto>>> Select(SearchParamsDto<SelectDiaSemanaFilterDto> searchParams)
             => await _mediator.Send(new SelectDiaSemanaQuery(searchParams));

    }
}
