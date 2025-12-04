using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Servicio
{
    public class DeleteServicioCommand : CommandBase
    {
        public DeleteServicioCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
