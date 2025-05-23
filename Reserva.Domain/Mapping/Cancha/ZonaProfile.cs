using AutoMapper;
using Reserva.Dto.Cancha.Zona;

namespace Reserva.Domain.Mapping.Zona
{
    public class ZonaProfile : Profile
    {
        public ZonaProfile()
        {
            CreateMap<Entity.Models.Zona, ZonaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Zona, CreateZonaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Zona, UpdateZonaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Zona, GetZonaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Zona, ListZonaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Zona, SelectComboZonaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Zona, SearchZonaDto>()
                .ReverseMap();
        }
    }
}
