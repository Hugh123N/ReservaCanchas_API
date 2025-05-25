using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.Proveedor
{
    public class DeleteProveedorCommand : CommandBase
    {
        public DeleteProveedorCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
