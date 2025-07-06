using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Cancha.IntentoLogin;
using Reserva.Domain.Queries.Cancha.IntentoLogin;
using Reserva.Dto.Cancha.IntentoLogin;

namespace Reserva.Application.Cancha
{
    public class IntentoLoginApplication : ApplicationBase, IIntentoLoginApplication
    {
        public IntentoLoginApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetIntentoLoginDto>> Create(CreateIntentoLoginDto createDto)
            => await _mediator.Send(new CreateIntentoLoginCommand(createDto));
        public async Task<ResponseDto<GetIntentoLoginDto>> Update(UpdateIntentoLoginDto updateDto)
            => await _mediator.Send(new UpdateIntentoLoginCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteIntentoLoginCommand(id));
        public async Task<ResponseDto<GetIntentoLoginDto>> Get(int id)
            => await _mediator.Send(new GetIntentoLoginQuery(id));
        public async Task<ResponseDto<IEnumerable<ListIntentoLoginDto>>> List(int id)
            => await _mediator.Send(new ListIntentoLoginQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchIntentoLoginDto>>> Search(SearchParamsDto<SearchIntentoLoginFilterDto> searchParams)
            => await _mediator.Send(new SearchIntentoLoginQuery(searchParams));
        public async Task<ResponseDto<SearchResultDto<SelectIntentoLoginDto>>> Select(SearchParamsDto<SelectIntentoLoginFilterDto> searchParams)
             => await _mediator.Send(new SelectIntentoLoginQuery(searchParams));

    }
}
