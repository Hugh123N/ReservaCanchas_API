using AutoMapper;
using Reserva.Dto.Dbo.Disponibilidad;

namespace Reserva.Domain.Mapping.Disponibilidad
{
    public class DisponibilidadProfile : Profile
    {
        public DisponibilidadProfile()
        {
            CreateMap<Entity.Disponibilidad, DisponibilidadDto>()
                .ReverseMap();

            CreateMap<Entity.Disponibilidad, CreateDisponibilidadDto>()
                .ReverseMap();

            CreateMap<Entity.Disponibilidad, UpdateDisponibilidadDto>()
                .ReverseMap();

            CreateMap<Entity.Disponibilidad, GetDisponibilidadDto>()
                .ReverseMap();

            CreateMap<Entity.Disponibilidad, ListDisponibilidadDto>()
                .ReverseMap();

            CreateMap<Entity.Disponibilidad, SelectComboDisponibilidadDto>()
                .ReverseMap();

            CreateMap<Entity.Disponibilidad, SearchDisponibilidadDto>()
                .ReverseMap();
        }
    }
}
