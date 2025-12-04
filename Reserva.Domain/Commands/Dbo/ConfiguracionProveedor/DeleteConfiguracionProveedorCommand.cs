using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.ConfiguracionProveedor
{
    public class DeleteConfiguracionProveedorCommand : CommandBase
    {
        public DeleteConfiguracionProveedorCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
