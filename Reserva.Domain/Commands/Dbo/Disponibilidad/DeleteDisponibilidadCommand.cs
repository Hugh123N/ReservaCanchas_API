using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Disponibilidad
{
    public class DeleteDisponibilidadCommand : CommandBase
    {
        public DeleteDisponibilidadCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
