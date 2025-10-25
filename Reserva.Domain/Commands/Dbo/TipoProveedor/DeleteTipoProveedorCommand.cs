using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.TipoProveedor
{
    public class DeleteTipoProveedorCommand : CommandBase
    {
        public DeleteTipoProveedorCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
