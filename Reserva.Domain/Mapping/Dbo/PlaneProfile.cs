using AutoMapper;
using Reserva.Dto.Dbo.Plane;

namespace Reserva.Domain.Mapping.Plane
{
    public class PlaneProfile : Profile
    {
        public PlaneProfile()
        {
            CreateMap<Entity.Plane, PlaneDto>()
                .ReverseMap();

            CreateMap<Entity.Plane, CreatePlaneDto>()
                .ReverseMap();

            CreateMap<Entity.Plane, UpdatePlaneDto>()
                .ReverseMap();

            CreateMap<Entity.Plane, GetPlaneDto>()
                .ReverseMap();

            CreateMap<Entity.Plane, ListPlaneDto>()
                .ForMember(dest => dest.PlanCaracteristicas, opt => opt.MapFrom(src => src.PlanCaracteristica != null ? src.PlanCaracteristica : null))
                .ForMember(dest => dest.PlanTarifa, opt => opt.MapFrom(src => src.PlanTarifa != null ? src.PlanTarifa : null))
                .ReverseMap();
        }
    }
}
