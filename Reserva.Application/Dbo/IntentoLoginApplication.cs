using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Dbo.IntentoLogin;
using Reserva.Domain.Queries.Dbo.IntentoLogin;
using Reserva.Dto.Dbo.IntentoLogin;

namespace Reserva.Application.Dbo
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
