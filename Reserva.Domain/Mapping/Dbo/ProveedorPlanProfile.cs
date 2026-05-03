using AutoMapper;
using Reserva.Dto.Dbo.ProveedorPlan;
using Reserva.Entity;

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

            CreateMap<Entity.ProveedorPlan, GetProveedorPlanCurrentDto>()
                .ForMember(dest => dest.Plan, opt => opt.MapFrom(src => src.IdPlaneNavigation))
                .ForMember(dest => dest.PlanTarifas, opt => opt.MapFrom(src => src.IdPlanTarifaNavigation))
                .ForMember(dest => dest.PlanCaracteristicas, opt => opt.MapFrom(src => src.IdPlaneNavigation != null ? src.IdPlaneNavigation.PlanCaracteristica : null))
                .ForMember(dest => dest.Limites, opt => opt.MapFrom(src => src.IdPlaneNavigation != null ? src.IdPlaneNavigation.PlanLimite : null))
                .ReverseMap();

            // Additional mappings for nested DTOs
            CreateMap<Entity.PlanLimite, PlanLimiteDto>()
                .ReverseMap();
        }
    }
}
