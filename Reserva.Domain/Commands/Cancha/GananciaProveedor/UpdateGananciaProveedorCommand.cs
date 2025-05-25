using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.GananciaProveedor;

namespace Reserva.Domain.Commands.Cancha.GananciaProveedor
{
    public class UpdateGananciaProveedorCommand : CommandBase<GetGananciaProveedorDto>
    {
        public UpdateGananciaProveedorCommand(UpdateGananciaProveedorDto updateDto) => UpdateDto = updateDto;
        public UpdateGananciaProveedorDto UpdateDto { get; set; }
    }
}
