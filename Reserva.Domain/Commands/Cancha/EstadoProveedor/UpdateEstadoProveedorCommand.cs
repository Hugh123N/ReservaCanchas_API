using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.EstadoProveedor;

namespace Reserva.Domain.Commands.Cancha.EstadoProveedor
{
    public class UpdateEstadoProveedorCommand : CommandBase<GetEstadoProveedorDto>
    {
        public UpdateEstadoProveedorCommand(UpdateEstadoProveedorDto updateDto) => UpdateDto = updateDto;
        public UpdateEstadoProveedorDto UpdateDto { get; set; }
    }
}
