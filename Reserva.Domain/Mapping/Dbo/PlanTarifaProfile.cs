using AutoMapper;
using Reserva.Dto.Dbo.ProveedorPlan;

namespace Reserva.Domain.Mapping.Dbo
{
    public class PlanTarifaProfile : Profile
    {
        public PlanTarifaProfile()
        {
            CreateMap<Entity.PlanTarifa, PlanTarifaDto>()
                .ReverseMap();

            CreateMap<Entity.PlanTarifa, GetPlanTarifaDto>()
                .ReverseMap();
        }
    }
}
