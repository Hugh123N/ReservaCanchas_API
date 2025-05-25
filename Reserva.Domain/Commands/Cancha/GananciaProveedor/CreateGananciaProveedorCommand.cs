using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.GananciaProveedor;

namespace Reserva.Domain.Commands.Cancha.GananciaProveedor
{
    public class CreateGananciaProveedorCommand : CommandBase<GetGananciaProveedorDto>
    {
        public CreateGananciaProveedorCommand(CreateGananciaProveedorDto createDto) => CreateDto = createDto;
        public CreateGananciaProveedorDto CreateDto { get; set; }
    }
}
