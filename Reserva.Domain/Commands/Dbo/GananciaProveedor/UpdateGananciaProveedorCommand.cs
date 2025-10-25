using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.GananciaProveedor;

namespace Reserva.Domain.Commands.Dbo.GananciaProveedor
{
    public class UpdateGananciaProveedorCommand : CommandBase<GetGananciaProveedorDto>
    {
        public UpdateGananciaProveedorCommand(UpdateGananciaProveedorDto updateDto) => UpdateDto = updateDto;
        public UpdateGananciaProveedorDto UpdateDto { get; set; }
    }
}
