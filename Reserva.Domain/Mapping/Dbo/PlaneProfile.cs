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
                .ReverseMap();
        }
    }
}
