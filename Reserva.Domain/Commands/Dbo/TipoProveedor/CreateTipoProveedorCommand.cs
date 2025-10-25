using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.TipoProveedor;

namespace Reserva.Domain.Commands.Dbo.TipoProveedor
{
    public class CreateTipoProveedorCommand : CommandBase<GetTipoProveedorDto>
    {
        public CreateTipoProveedorCommand(CreateTipoProveedorDto createDto) => CreateDto = createDto;
        public CreateTipoProveedorDto CreateDto { get; set; }
    }
}
