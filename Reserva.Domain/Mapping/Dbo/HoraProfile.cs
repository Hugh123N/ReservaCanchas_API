using AutoMapper;
using Reserva.Dto.Dbo.Hora;

namespace Reserva.Domain.Mapping.Hora
{
    public class HoraProfile : Profile
    {
        public HoraProfile()
        {
            CreateMap<Entity.Hora, HoraDto>()
                .ReverseMap();

            CreateMap<Entity.Hora, GetHoraDto>()
                .ReverseMap();

            CreateMap<Entity.Hora, ListHoraDto>()
                .ReverseMap();

            CreateMap<Entity.Hora, SelectComboHoraDto>()
                .ReverseMap();

            CreateMap<Entity.Hora, SearchHoraDto>()
                .ReverseMap();
        }
    }
}
