using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.EstadoProveedor
{
    public class DeleteEstadoProveedorCommand : CommandBase
    {
        public DeleteEstadoProveedorCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
