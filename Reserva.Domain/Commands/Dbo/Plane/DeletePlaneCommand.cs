using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Plane
{
    public class DeletePlaneCommand : CommandBase
    {
        public DeletePlaneCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
