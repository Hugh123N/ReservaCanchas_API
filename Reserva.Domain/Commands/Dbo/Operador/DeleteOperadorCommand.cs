using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Operador
{
    public class DeleteOperadorCommand : CommandBase
    {
        public DeleteOperadorCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
