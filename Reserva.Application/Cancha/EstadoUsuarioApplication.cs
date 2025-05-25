using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Cancha.EstadoUsuario;
using Reserva.Domain.Queries.Cancha.EstadoUsuario;
using Reserva.Dto.Cancha.EstadoUsuario;

namespace Reserva.Application.Cancha
{
    public class EstadoUsuarioApplication : ApplicationBase, IEstadoUsuarioApplication
    {
        public EstadoUsuarioApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetEstadoUsuarioDto>> Create(CreateEstadoUsuarioDto createDto)
            => await _mediator.Send(new CreateEstadoUsuarioCommand(createDto));
        public async Task<ResponseDto<GetEstadoUsuarioDto>> Update(UpdateEstadoUsuarioDto updateDto)
            => await _mediator.Send(new UpdateEstadoUsuarioCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteEstadoUsuarioCommand(id));
        public async Task<ResponseDto<GetEstadoUsuarioDto>> Get(int id)
            => await _mediator.Send(new GetEstadoUsuarioQuery(id));
        public async Task<ResponseDto<IEnumerable<ListEstadoUsuarioDto>>> List(int id)
            => await _mediator.Send(new ListEstadoUsuarioQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchEstadoUsuarioDto>>> Search(SearchParamsDto<SearchEstadoUsuarioFilterDto> searchParams)
            => await _mediator.Send(new SearchEstadoUsuarioQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboEstadoUsuarioDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboEstadoUsuarioQuery());
        public async Task<ResponseDto<SearchResultDto<SelectEstadoUsuarioDto>>> Select(SearchParamsDto<SelectEstadoUsuarioFilterDto> searchParams)
             => await _mediator.Send(new SelectEstadoUsuarioQuery(searchParams));

    }
}
