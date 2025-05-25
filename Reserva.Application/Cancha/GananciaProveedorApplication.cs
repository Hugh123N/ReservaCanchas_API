using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Cancha.GananciaProveedor;
using Reserva.Domain.Queries.Cancha.GananciaProveedor;
using Reserva.Dto.Cancha.GananciaProveedor;

namespace Reserva.Application.Cancha
{
    public class GananciaProveedorApplication : ApplicationBase, IGananciaProveedorApplication
    {
        public GananciaProveedorApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetGananciaProveedorDto>> Create(CreateGananciaProveedorDto createDto)
            => await _mediator.Send(new CreateGananciaProveedorCommand(createDto));
        public async Task<ResponseDto<GetGananciaProveedorDto>> Update(UpdateGananciaProveedorDto updateDto)
            => await _mediator.Send(new UpdateGananciaProveedorCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteGananciaProveedorCommand(id));
        public async Task<ResponseDto<GetGananciaProveedorDto>> Get(int id)
            => await _mediator.Send(new GetGananciaProveedorQuery(id));
        public async Task<ResponseDto<IEnumerable<ListGananciaProveedorDto>>> List(int id)
            => await _mediator.Send(new ListGananciaProveedorQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchGananciaProveedorDto>>> Search(SearchParamsDto<SearchGananciaProveedorFilterDto> searchParams)
            => await _mediator.Send(new SearchGananciaProveedorQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboGananciaProveedorDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboGananciaProveedorQuery());
        public async Task<ResponseDto<SearchResultDto<SelectGananciaProveedorDto>>> Select(SearchParamsDto<SelectGananciaProveedorFilterDto> searchParams)
             => await _mediator.Send(new SelectGananciaProveedorQuery(searchParams));

    }
}
