using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.TipoProveedor;

namespace Reserva.Domain.Commands.Cancha.TipoProveedor
{
    public class UpdateTipoProveedorCommand : CommandBase<GetTipoProveedorDto>
    {
        public UpdateTipoProveedorCommand(UpdateTipoProveedorDto updateDto) => UpdateDto = updateDto;
        public UpdateTipoProveedorDto UpdateDto { get; set; }
    }
}
