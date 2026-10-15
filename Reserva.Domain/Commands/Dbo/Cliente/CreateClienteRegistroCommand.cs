using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Cliente;

namespace Reserva.Domain.Commands.Dbo.Cliente
{
    public class CreateClienteRegistroCommand : CommandBase<GetClienteDto>
    {
        public CreateClienteRegistroCommand(CreateClienteRegistroDto createDto) => CreateDto = createDto;
        public CreateClienteRegistroDto CreateDto { get; set; }
    }
}
