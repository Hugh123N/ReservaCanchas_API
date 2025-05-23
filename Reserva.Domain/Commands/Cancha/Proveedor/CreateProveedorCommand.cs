using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Proveedor;

namespace Reserva.Domain.Commands.Cancha.Proveedor
{
    public class CreateProveedorCommand : CommandBase<GetProveedorDto>
    {
        public CreateProveedorCommand(CreateProveedorDto createDto) => CreateDto = createDto;
        public CreateProveedorDto CreateDto { get; set; }
    }
}
