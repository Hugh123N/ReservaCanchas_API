using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.ProveedorPlan
{
    public class CancelAutoRenewCommand : CommandBase<ResponseDto>
    {
        public CancelAutoRenewCommand(int idProveedorPlan) => IdProveedorPlan = idProveedorPlan;
        public int IdProveedorPlan { get; set; }
    }
}