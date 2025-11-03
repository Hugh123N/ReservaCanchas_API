using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Proveedor
{
    public class DeleteProveedorCommand : CommandBase
    {
        public DeleteProveedorCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
