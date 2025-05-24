using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Cancha.Ubigeo;
using Reserva.Domain.Queries.Cancha.Ubigeo;
using Reserva.Dto.Cancha.Ubigeo;

namespace Reserva.Application.Cancha
{
    public class UbigeoApplication : ApplicationBase, IUbigeoApplication
    {
        public UbigeoApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetUbigeoDto>> Create(CreateUbigeoDto createDto)
            => await _mediator.Send(new CreateUbigeoCommand(createDto));
        public async Task<ResponseDto<GetUbigeoDto>> Update(UpdateUbigeoDto updateDto)
            => await _mediator.Send(new UpdateUbigeoCommand(updateDto));
        public async Task<ResponseDto> Delete(string id)
            => await _mediator.Send(new DeleteUbigeoCommand(id));
        public async Task<ResponseDto<GetUbigeoDto>> Get(string id)
            => await _mediator.Send(new GetUbigeoQuery(id));
        public async Task<ResponseDto<IEnumerable<DepartamentoDto>>> List()
            => await _mediator.Send(new ListUbigeoQuery());
        public async Task<ResponseDto<SearchResultDto<SelectUbigeoDto>>> Select(SearchParamsDto<SelectUbigeoFilterDto> searchParams)
             => await _mediator.Send(new SelectUbigeoQuery(searchParams));

    }
}
