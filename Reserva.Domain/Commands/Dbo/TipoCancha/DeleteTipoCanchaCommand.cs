using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.TipoCancha
{
    public class DeleteTipoCanchaCommand : CommandBase
    {
        public DeleteTipoCanchaCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
