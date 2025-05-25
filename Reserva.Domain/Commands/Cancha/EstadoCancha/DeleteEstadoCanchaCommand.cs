using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.EstadoCancha
{
    public class DeleteEstadoCanchaCommand : CommandBase
    {
        public DeleteEstadoCanchaCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
