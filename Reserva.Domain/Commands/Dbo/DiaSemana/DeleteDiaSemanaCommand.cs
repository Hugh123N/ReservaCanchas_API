using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.DiaSemana
{
    public class DeleteDiaSemanaCommand : CommandBase
    {
        public DeleteDiaSemanaCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
