using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Cancha.Usuario;
using Reserva.Domain.Queries.Cancha.Usuario;
using Reserva.Dto.Cancha.Usuario;

namespace Reserva.Application.Cancha
{
    public class UsuarioApplication : ApplicationBase, IUsuarioApplication
    {
        public UsuarioApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetUsuarioDto>> Create(CreateUsuarioDto createDto)
            => await _mediator.Send(new CreateUsuarioCommand(createDto));
        public async Task<ResponseDto<GetUsuarioDto>> Update(UpdateUsuarioDto updateDto)
            => await _mediator.Send(new UpdateUsuarioCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteUsuarioCommand(id));
        public async Task<ResponseDto<GetUsuarioDto>> Get(int id)
            => await _mediator.Send(new GetUsuarioQuery(id));
        public async Task<ResponseDto<IEnumerable<ListUsuarioDto>>> List(int id)
            => await _mediator.Send(new ListUsuarioQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchUsuarioDto>>> Search(SearchParamsDto<SearchUsuarioFilterDto> searchParams)
            => await _mediator.Send(new SearchUsuarioQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboUsuarioDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboUsuarioQuery());
        public async Task<ResponseDto<SearchResultDto<SelectUsuarioDto>>> Select(SearchParamsDto<SelectUsuarioFilterDto> searchParams)
             => await _mediator.Send(new SelectUsuarioQuery(searchParams));

    }
}
