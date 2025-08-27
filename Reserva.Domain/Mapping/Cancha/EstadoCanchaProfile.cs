using AutoMapper;
using Reserva.Dto.Cancha.EstadoCancha;

namespace Reserva.Domain.Mapping.EstadoCancha
{
    public class EstadoCanchaProfile : Profile
    {
        public EstadoCanchaProfile()
        {
            CreateMap<Entity.EstadoCancha, EstadoCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.EstadoCancha, CreateEstadoCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.EstadoCancha, UpdateEstadoCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.EstadoCancha, GetEstadoCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.EstadoCancha, ListEstadoCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.EstadoCancha, SelectComboEstadoCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.EstadoCancha, SearchEstadoCanchaDto>()
                .ReverseMap();
        }
    }
}
