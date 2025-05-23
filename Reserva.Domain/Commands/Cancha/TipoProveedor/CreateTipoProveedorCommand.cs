using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.TipoProveedor;

namespace Reserva.Domain.Commands.Cancha.TipoProveedor
{
    public class CreateTipoProveedorCommand : CommandBase<GetTipoProveedorDto>
    {
        public CreateTipoProveedorCommand(CreateTipoProveedorDto createDto) => CreateDto = createDto;
        public CreateTipoProveedorDto CreateDto { get; set; }
    }
}
