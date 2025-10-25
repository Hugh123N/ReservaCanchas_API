using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.IntentoLogin
{
    public class DeleteIntentoLoginCommand : CommandBase
    {
        public DeleteIntentoLoginCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
