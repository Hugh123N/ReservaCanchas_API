using AutoMapper;
using Reserva.Dto.Cancha.Disponibilidad;

namespace Reserva.Domain.Mapping.Disponibilidad
{
    public class DisponibilidadProfile : Profile
    {
        public DisponibilidadProfile()
        {
            CreateMap<Entity.Models.Disponibilidad, DisponibilidadDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Disponibilidad, CreateDisponibilidadDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Disponibilidad, UpdateDisponibilidadDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Disponibilidad, GetDisponibilidadDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Disponibilidad, ListDisponibilidadDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Disponibilidad, SelectComboDisponibilidadDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Disponibilidad, SearchDisponibilidadDto>()
                .ReverseMap();
        }
    }
}
