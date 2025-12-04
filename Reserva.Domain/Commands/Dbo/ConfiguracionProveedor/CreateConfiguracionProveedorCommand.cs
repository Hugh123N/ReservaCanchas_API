using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.ConfiguracionProveedor;

namespace Reserva.Domain.Commands.Dbo.ConfiguracionProveedor
{
    public class CreateConfiguracionProveedorCommand : CommandBase<GetConfiguracionProveedorDto>
    {
        public CreateConfiguracionProveedorCommand(CreateConfiguracionProveedorDto createDto) => CreateDto = createDto;
        public CreateConfiguracionProveedorDto CreateDto { get; set; }
    }
}
