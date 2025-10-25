using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.GananciaProveedor;

namespace Reserva.Domain.Commands.Dbo.GananciaProveedor
{
    public class CreateGananciaProveedorCommand : CommandBase<GetGananciaProveedorDto>
    {
        public CreateGananciaProveedorCommand(CreateGananciaProveedorDto createDto) => CreateDto = createDto;
        public CreateGananciaProveedorDto CreateDto { get; set; }
    }
}
