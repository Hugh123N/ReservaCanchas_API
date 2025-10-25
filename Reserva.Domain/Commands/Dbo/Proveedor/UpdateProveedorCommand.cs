using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Proveedor;

namespace Reserva.Domain.Commands.Dbo.Proveedor
{
    public class UpdateProveedorCommand : CommandBase<GetProveedorDto>
    {
        public UpdateProveedorCommand(UpdateProveedorDto updateDto) => UpdateDto = updateDto;
        public UpdateProveedorDto UpdateDto { get; set; }
    }
}
