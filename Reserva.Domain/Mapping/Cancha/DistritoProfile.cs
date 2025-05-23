using AutoMapper;
using Reserva.Dto.Cancha.Distrito;

namespace Reserva.Domain.Mapping.Distrito
{
    public class DistritoProfile : Profile
    {
        public DistritoProfile()
        {
            CreateMap<Entity.Models.Distrito, DistritoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Distrito, CreateDistritoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Distrito, UpdateDistritoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Distrito, GetDistritoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Distrito, ListDistritoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Distrito, SelectComboDistritoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Distrito, SearchDistritoDto>()
                .ReverseMap();
        }
    }
}
