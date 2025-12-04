using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.HorarioCancha
{
    public class DeleteHorarioCanchaCommand : CommandBase
    {
        public DeleteHorarioCanchaCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
