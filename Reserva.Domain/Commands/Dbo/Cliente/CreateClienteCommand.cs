using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Cliente;

namespace Reserva.Domain.Commands.Dbo.Cliente
{
    public class CreateClienteCommand : CommandBase<GetClienteDto>
    {
        public CreateClienteCommand(CreateClienteDto createDto) => CreateDto = createDto;
        public CreateClienteDto CreateDto { get; set; }
    }
}
