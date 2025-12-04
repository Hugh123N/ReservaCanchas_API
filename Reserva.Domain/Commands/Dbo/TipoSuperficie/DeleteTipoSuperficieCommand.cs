using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.TipoSuperficie
{
    public class DeleteTipoSuperficieCommand : CommandBase
    {
        public DeleteTipoSuperficieCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
