using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.EstadoProveedor;

namespace Reserva.Domain.Commands.Cancha.EstadoProveedor
{
    public class CreateEstadoProveedorCommand : CommandBase<GetEstadoProveedorDto>
    {
        public CreateEstadoProveedorCommand(CreateEstadoProveedorDto createDto) => CreateDto = createDto;
        public CreateEstadoProveedorDto CreateDto { get; set; }
    }
}
