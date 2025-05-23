using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Cancha.TipoProveedor;
using Reserva.Domain.Queries.Cancha.TipoProveedor;
using Reserva.Dto.Cancha.TipoProveedor;

namespace Reserva.Application.Cancha
{
    public class TipoProveedorApplication : ApplicationBase, ITipoProveedorApplication
    {
        public TipoProveedorApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetTipoProveedorDto>> Create(CreateTipoProveedorDto createDto)
            => await _mediator.Send(new CreateTipoProveedorCommand(createDto));
        public async Task<ResponseDto<GetTipoProveedorDto>> Update(UpdateTipoProveedorDto updateDto)
            => await _mediator.Send(new UpdateTipoProveedorCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteTipoProveedorCommand(id));
        public async Task<ResponseDto<GetTipoProveedorDto>> Get(int id)
            => await _mediator.Send(new GetTipoProveedorQuery(id));
        public async Task<ResponseDto<IEnumerable<ListTipoProveedorDto>>> List(int id)
            => await _mediator.Send(new ListTipoProveedorQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchTipoProveedorDto>>> Search(SearchParamsDto<SearchTipoProveedorFilterDto> searchParams)
            => await _mediator.Send(new SearchTipoProveedorQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboTipoProveedorDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboTipoProveedorQuery());
        public async Task<ResponseDto<SearchResultDto<SelectTipoProveedorDto>>> Select(SearchParamsDto<SelectTipoProveedorFilterDto> searchParams)
             => await _mediator.Send(new SelectTipoProveedorQuery(searchParams));

    }
}
