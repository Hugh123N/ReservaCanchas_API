using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Cancha;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Cancha.Proveedor;
using Reserva.Domain.Queries.Cancha.Proveedor;
using Reserva.Dto.Cancha.Proveedor;

namespace Reserva.Application.Cancha
{
    public class ProveedorApplication : ApplicationBase, IProveedorApplication
    {
        public ProveedorApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetProveedorDto>> Create(CreateProveedorDto createDto)
            => await _mediator.Send(new CreateProveedorCommand(createDto));
        public async Task<ResponseDto<GetProveedorDto>> Update(UpdateProveedorDto updateDto)
            => await _mediator.Send(new UpdateProveedorCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteProveedorCommand(id));
        public async Task<ResponseDto<GetProveedorDto>> Get(int id)
            => await _mediator.Send(new GetProveedorQuery(id));
        public async Task<ResponseDto<IEnumerable<ListProveedorDto>>> List(int id)
            => await _mediator.Send(new ListProveedorQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchProveedorDto>>> Search(SearchParamsDto<SearchProveedorFilterDto> searchParams)
            => await _mediator.Send(new SearchProveedorQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboProveedorDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboProveedorQuery());
        public async Task<ResponseDto<SearchResultDto<SelectProveedorDto>>> Select(SearchParamsDto<SelectProveedorFilterDto> searchParams)
             => await _mediator.Send(new SelectProveedorQuery(searchParams));

    }
}
