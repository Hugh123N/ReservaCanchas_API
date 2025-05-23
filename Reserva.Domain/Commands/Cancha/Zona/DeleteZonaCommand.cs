using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.Zona
{
    public class DeleteZonaCommand : CommandBase
    {
        public DeleteZonaCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
