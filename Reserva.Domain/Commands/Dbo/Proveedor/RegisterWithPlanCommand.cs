using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Proveedor;
using Reserva.Dto.User;

namespace Reserva.Domain.Commands.Dbo.Proveedor
{
    /// <summary>
    /// Command para registro de proveedor con plan gratuito.
    /// Orquesta: Crear Proveedor + Crear ProveedorPlan + Login
    /// </summary>
    public class RegisterWithPlanCommand : CommandBase<LoginResultDto>
    {
        public RegisterWithPlanCommand(RegisterWithPlanDto dto) => Dto = dto;
        public RegisterWithPlanDto Dto { get; set; }
    }
}
