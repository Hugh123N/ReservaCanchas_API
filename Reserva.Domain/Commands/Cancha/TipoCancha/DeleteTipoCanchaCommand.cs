using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.TipoCancha
{
    public class DeleteTipoCanchaCommand : CommandBase
    {
        public DeleteTipoCanchaCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
