using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.DetallePago
{
    public class DeleteDetallePagoCommand : CommandBase
    {
        public DeleteDetallePagoCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
