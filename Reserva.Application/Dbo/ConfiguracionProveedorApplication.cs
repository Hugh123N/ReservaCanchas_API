using MediatR;
using Reserva.Dto.Base;
using Reserva.Application.Abstractions.Dbo;
using Reserva.Application.Base;
using Reserva.Domain.Commands.Dbo.ConfiguracionProveedor;
using Reserva.Domain.Queries.Dbo.ConfiguracionProveedor;
using Reserva.Dto.Dbo.ConfiguracionProveedor;

namespace Reserva.Application.Dbo
{
    public class ConfiguracionProveedorApplication : ApplicationBase, IConfiguracionProveedorApplication
    {
        public ConfiguracionProveedorApplication(IMediator mediator) : base(mediator)
        {

        }

        public async Task<ResponseDto<GetConfiguracionProveedorDto>> Create(CreateConfiguracionProveedorDto createDto)
            => await _mediator.Send(new CreateConfiguracionProveedorCommand(createDto));
        public async Task<ResponseDto<GetConfiguracionProveedorDto>> Update(UpdateConfiguracionProveedorDto updateDto)
            => await _mediator.Send(new UpdateConfiguracionProveedorCommand(updateDto));
        public async Task<ResponseDto> Delete(int id)
            => await _mediator.Send(new DeleteConfiguracionProveedorCommand(id));
        public async Task<ResponseDto<GetConfiguracionProveedorDto>> Get(int id)
            => await _mediator.Send(new GetConfiguracionProveedorQuery(id));
        public async Task<ResponseDto<IEnumerable<ListConfiguracionProveedorDto>>> List(int id)
            => await _mediator.Send(new ListConfiguracionProveedorQuery(id));
        public async Task<ResponseDto<SearchResultDto<SearchConfiguracionProveedorDto>>> Search(SearchParamsDto<SearchConfiguracionProveedorFilterDto> searchParams)
            => await _mediator.Send(new SearchConfiguracionProveedorQuery(searchParams));
        public async Task<ResponseDto<IEnumerable<SelectComboConfiguracionProveedorDto>>> SelectCombo()
            => await _mediator.Send(new SelectComboConfiguracionProveedorQuery());

    }
}
