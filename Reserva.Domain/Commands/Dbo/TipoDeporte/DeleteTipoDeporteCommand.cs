using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.TipoDeporte
{
    public class DeleteTipoDeporteCommand : CommandBase
    {
        public DeleteTipoDeporteCommand(int id) => Id = id;
        public int Id { get; set; }
    }
}
