using AutoMapper;
using Reserva.Dto.Dbo.ProveedorPlan;

namespace Reserva.Domain.Mapping.ProveedorPlan
{
    public class ProveedorPlanProfile : Profile
    {
        public ProveedorPlanProfile()
        {
            CreateMap<Entity.ProveedorPlan, ProveedorPlanDto>()
                .ReverseMap();

            CreateMap<Entity.ProveedorPlan, CreateProveedorPlanDto>()
                .ReverseMap();

            CreateMap<Entity.ProveedorPlan, UpdateProveedorPlanDto>()
                .ReverseMap();

            CreateMap<Entity.ProveedorPlan, GetProveedorPlanDto>()
                .ReverseMap();

            CreateMap<Entity.ProveedorPlan, ListProveedorPlanDto>()
                .ReverseMap();

            CreateMap<Entity.ProveedorPlan, SearchProveedorPlanDto>()
                .ReverseMap();
        }
    }
}
