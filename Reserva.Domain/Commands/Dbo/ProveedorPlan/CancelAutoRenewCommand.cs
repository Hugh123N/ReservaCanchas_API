using Reserva.Domain.Commands.Base;
using Reserva.Dto.Base;

namespace Reserva.Domain.Commands.Dbo.ProveedorPlan
{
    public class CancelAutoRenewCommand : CommandBase
    {
        public CancelAutoRenewCommand(int idProveedorPlan) => IdProveedorPlan = idProveedorPlan;
        public int IdProveedorPlan { get; set; }
    }
}