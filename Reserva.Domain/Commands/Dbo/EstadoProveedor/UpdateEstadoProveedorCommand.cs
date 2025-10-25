using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.EstadoProveedor;

namespace Reserva.Domain.Commands.Dbo.EstadoProveedor
{
    public class UpdateEstadoProveedorCommand : CommandBase<GetEstadoProveedorDto>
    {
        public UpdateEstadoProveedorCommand(UpdateEstadoProveedorDto updateDto) => UpdateDto = updateDto;
        public UpdateEstadoProveedorDto UpdateDto { get; set; }
    }
}
