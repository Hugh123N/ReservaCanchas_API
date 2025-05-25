using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.EstadoProveedor
{
    public class DeleteEstadoProveedorCommand : CommandBase
    {
        public DeleteEstadoProveedorCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
