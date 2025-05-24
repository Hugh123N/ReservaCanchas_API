using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.Ubigeo
{
    public class DeleteUbigeoCommand : CommandBase
    {
        public DeleteUbigeoCommand(string id) => Id = id;
        public string Id { get; set; }
    }
}
