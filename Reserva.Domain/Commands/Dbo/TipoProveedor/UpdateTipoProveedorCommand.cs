using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.TipoProveedor;

namespace Reserva.Domain.Commands.Dbo.TipoProveedor
{
    public class UpdateTipoProveedorCommand : CommandBase<GetTipoProveedorDto>
    {
        public UpdateTipoProveedorCommand(UpdateTipoProveedorDto updateDto) => UpdateDto = updateDto;
        public UpdateTipoProveedorDto UpdateDto { get; set; }
    }
}
