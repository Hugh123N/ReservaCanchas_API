using Reserva.Domain.Queries.Base;
using Reserva.Dto.Dbo.ProveedorPlan;

namespace Reserva.Domain.Queries.Dbo.ProveedorPlan
{
    public class CalculateProrationQuery : QueryBase<CalculateProrationResponseDto>
    {
        public CalculateProrationQuery(CalculateProrationDto dto) => Dto = dto;
        public CalculateProrationDto Dto { get; set; }
    }
}
