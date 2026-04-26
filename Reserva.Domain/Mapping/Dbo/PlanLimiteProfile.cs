using AutoMapper;
using Reserva.Dto.Dbo.PlanLimite;

namespace Reserva.Domain.Mapping.PlanLimite
{
    public class PlanLimiteProfile : Profile
    {
        public PlanLimiteProfile()
        {
            CreateMap<Entity.PlanLimite, PlanLimiteDto>()
                .ReverseMap();

            CreateMap<Entity.PlanLimite, CreatePlanLimiteDto>()
                .ReverseMap();

            CreateMap<Entity.PlanLimite, UpdatePlanLimiteDto>()
                .ReverseMap();

            CreateMap<Entity.PlanLimite, GetPlanLimiteDto>()
                .ReverseMap();

            CreateMap<Entity.PlanLimite, ListPlanLimiteDto>()
                .ReverseMap();
        }
    }
}
