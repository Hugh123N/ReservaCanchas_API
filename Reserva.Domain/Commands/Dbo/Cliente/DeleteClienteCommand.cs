using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Cliente
{
    public class DeleteClienteCommand : CommandBase
    {
        public DeleteClienteCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
