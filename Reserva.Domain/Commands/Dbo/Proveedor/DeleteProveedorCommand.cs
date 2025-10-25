using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Proveedor
{
    public class DeleteProveedorCommand : CommandBase
    {
        public DeleteProveedorCommand(Guid id) => Id = id;
        public Guid Id { get; set; }
    }
}
