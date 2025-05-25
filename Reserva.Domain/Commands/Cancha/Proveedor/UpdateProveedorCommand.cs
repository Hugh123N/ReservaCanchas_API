using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Proveedor;

namespace Reserva.Domain.Commands.Cancha.Proveedor
{
    public class UpdateProveedorCommand : CommandBase<GetProveedorDto>
    {
        public UpdateProveedorCommand(UpdateProveedorDto updateDto) => UpdateDto = updateDto;
        public UpdateProveedorDto UpdateDto { get; set; }
    }
}
