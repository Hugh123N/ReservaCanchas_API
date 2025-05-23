using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.GananciaProveedor
{
    public class DeleteGananciaProveedorCommand : CommandBase
    {
        public DeleteGananciaProveedorCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
