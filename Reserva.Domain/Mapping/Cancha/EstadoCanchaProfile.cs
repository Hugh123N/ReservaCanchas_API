using AutoMapper;
using Reserva.Dto.Cancha.EstadoCancha;

namespace Reserva.Domain.Mapping.EstadoCancha
{
    public class EstadoCanchaProfile : Profile
    {
        public EstadoCanchaProfile()
        {
            CreateMap<Entity.Models.EstadoCancha, EstadoCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.EstadoCancha, CreateEstadoCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.EstadoCancha, UpdateEstadoCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.EstadoCancha, GetEstadoCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.EstadoCancha, ListEstadoCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.EstadoCancha, SelectComboEstadoCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.EstadoCancha, SearchEstadoCanchaDto>()
                .ReverseMap();
        }
    }
}
