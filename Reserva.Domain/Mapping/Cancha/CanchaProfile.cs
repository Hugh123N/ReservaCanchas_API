using AutoMapper;
using Reserva.Dto.Cancha.Cancha;

namespace Reserva.Domain.Mapping.Cancha
{
    public class CanchaProfile : Profile
    {
        public CanchaProfile()
        {
            CreateMap<Entity.Models.Cancha, CanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Cancha, CreateCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Cancha, UpdateCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Cancha, GetCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Cancha, ListCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Cancha, SelectComboCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Cancha, SearchCanchaDto>()
                .ReverseMap();
        }
    }
}
