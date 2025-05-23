using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Cancha.Distrito;
using Reserva.Domain.Queries.Cancha.Distrito;
using Reserva.Dto.Cancha.Distrito;

namespace Reserva.Application.Cancha
{
    public class DistritoApplication : ApplicationBase, IDistritoApplication
    {
        public DistritoApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetDistritoDto>> Create(CreateDistritoDto createDto)
            => await _mediator.Send(new CreateDistritoCommand(createDto));
        public async Task<ResponseDto<GetDistritoDto>> Update(UpdateDistritoDto updateDto)
            => await _mediator.Send(new UpdateDistritoCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteDistritoCommand(id));
        public async Task<ResponseDto<GetDistritoDto>> Get(int id)
            => await _mediator.Send(new GetDistritoQuery(id));
        public async Task<ResponseDto<IEnumerable<ListDistritoDto>>> List(int id)
            => await _mediator.Send(new ListDistritoQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchDistritoDto>>> Search(SearchParamsDto<SearchDistritoFilterDto> searchParams)
            => await _mediator.Send(new SearchDistritoQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboDistritoDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboDistritoQuery());
        public async Task<ResponseDto<SearchResultDto<SelectDistritoDto>>> Select(SearchParamsDto<SelectDistritoFilterDto> searchParams)
             => await _mediator.Send(new SelectDistritoQuery(searchParams));

    }
}
