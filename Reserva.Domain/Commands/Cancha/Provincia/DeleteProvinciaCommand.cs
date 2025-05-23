using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.Provincia
{
    public class DeleteProvinciaCommand : CommandBase
    {
        public DeleteProvinciaCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
