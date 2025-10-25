using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Dbo.EstadoProveedor;
using Reserva.Domain.Queries.Dbo.EstadoProveedor;
using Reserva.Dto.Dbo.EstadoProveedor;

namespace Reserva.Application.Dbo
{
    public class EstadoProveedorApplication : ApplicationBase, IEstadoProveedorApplication
    {
        public EstadoProveedorApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetEstadoProveedorDto>> Create(CreateEstadoProveedorDto createDto)
            => await _mediator.Send(new CreateEstadoProveedorCommand(createDto));
        public async Task<ResponseDto<GetEstadoProveedorDto>> Update(UpdateEstadoProveedorDto updateDto)
            => await _mediator.Send(new UpdateEstadoProveedorCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteEstadoProveedorCommand(id));
        public async Task<ResponseDto<GetEstadoProveedorDto>> Get(int id)
            => await _mediator.Send(new GetEstadoProveedorQuery(id));
        public async Task<ResponseDto<IEnumerable<ListEstadoProveedorDto>>> List(int id)
            => await _mediator.Send(new ListEstadoProveedorQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchEstadoProveedorDto>>> Search(SearchParamsDto<SearchEstadoProveedorFilterDto> searchParams)
            => await _mediator.Send(new SearchEstadoProveedorQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboEstadoProveedorDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboEstadoProveedorQuery());
        public async Task<ResponseDto<SearchResultDto<SelectEstadoProveedorDto>>> Select(SearchParamsDto<SelectEstadoProveedorFilterDto> searchParams)
             => await _mediator.Send(new SelectEstadoProveedorQuery(searchParams));

    }
}
