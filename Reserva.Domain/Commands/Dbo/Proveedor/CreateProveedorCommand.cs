using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Proveedor;

namespace Reserva.Domain.Commands.Dbo.Proveedor
{
    public class CreateProveedorCommand : CommandBase<GetProveedorDto>
    {
        public CreateProveedorCommand(CreateProveedorDto createDto) => CreateDto = createDto;
        public CreateProveedorDto CreateDto { get; set; }
    }
}
