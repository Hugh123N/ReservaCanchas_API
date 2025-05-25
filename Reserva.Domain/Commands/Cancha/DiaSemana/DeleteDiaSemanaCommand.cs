using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.DiaSemana
{
    public class DeleteDiaSemanaCommand : CommandBase
    {
        public DeleteDiaSemanaCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
