using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.EstadoProveedor;

namespace Reserva.Domain.Commands.Dbo.EstadoProveedor
{
    public class CreateEstadoProveedorCommand : CommandBase<GetEstadoProveedorDto>
    {
        public CreateEstadoProveedorCommand(CreateEstadoProveedorDto createDto) => CreateDto = createDto;
        public CreateEstadoProveedorDto CreateDto { get; set; }
    }
}
