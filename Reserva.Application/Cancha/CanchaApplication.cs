using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Cancha.Cancha;
using Reserva.Domain.Queries.Cancha.Cancha;
using Reserva.Dto.Cancha.Cancha;

namespace Reserva.Application.Cancha
{
    public class CanchaApplication : ApplicationBase, ICanchaApplication
    {
        public CanchaApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetCanchaDto>> Create(CreateCanchaDto createDto)
            => await _mediator.Send(new CreateCanchaCommand(createDto));
        public async Task<ResponseDto<GetCanchaDto>> Update(UpdateCanchaDto updateDto)
            => await _mediator.Send(new UpdateCanchaCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteCanchaCommand(id));
        public async Task<ResponseDto<GetCanchaDto>> Get(int id)
            => await _mediator.Send(new GetCanchaQuery(id));
        public async Task<ResponseDto<IEnumerable<ListCanchaDto>>> List(int id)
            => await _mediator.Send(new ListCanchaQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchCanchaDto>>> Search(SearchParamsDto<SearchCanchaFilterDto> searchParams)
            => await _mediator.Send(new SearchCanchaQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboCanchaDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboCanchaQuery());
        public async Task<ResponseDto<SearchResultDto<SelectCanchaDto>>> Select(SearchParamsDto<SelectCanchaFilterDto> searchParams)
             => await _mediator.Send(new SelectCanchaQuery(searchParams));

    }
}
