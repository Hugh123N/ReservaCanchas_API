using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Cliente;

namespace Reserva.Domain.Commands.Dbo.Cliente
{
    public class UpdateClienteCommand : CommandBase<GetClienteDto>
    {
        public UpdateClienteCommand(UpdateClienteDto updateDto) => UpdateDto = updateDto;
        public UpdateClienteDto UpdateDto { get; set; }
    }
}
