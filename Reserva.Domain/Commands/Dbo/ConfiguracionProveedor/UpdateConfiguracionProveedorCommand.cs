using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.ConfiguracionProveedor;

namespace Reserva.Domain.Commands.Dbo.ConfiguracionProveedor
{
    public class UpdateConfiguracionProveedorCommand : CommandBase<GetConfiguracionProveedorDto>
    {
        public UpdateConfiguracionProveedorCommand(UpdateConfiguracionProveedorDto updateDto) => UpdateDto = updateDto;
        public UpdateConfiguracionProveedorDto UpdateDto { get; set; }
    }
}
