using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.DetalleReserva
{
    public class DeleteDetalleReservaCommand : CommandBase
    {
        public DeleteDetalleReservaCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
