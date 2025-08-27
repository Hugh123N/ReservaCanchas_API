using AutoMapper;
using Reserva.Dto.Cancha.EstadoReserva;

namespace Reserva.Domain.Mapping.EstadoReserva
{
    public class EstadoReservaProfile : Profile
    {
        public EstadoReservaProfile()
        {
            CreateMap<Entity.EstadoReserva, EstadoReservaDto>()
                .ReverseMap();

            CreateMap<Entity.EstadoReserva, CreateEstadoReservaDto>()
                .ReverseMap();

            CreateMap<Entity.EstadoReserva, UpdateEstadoReservaDto>()
                .ReverseMap();

            CreateMap<Entity.EstadoReserva, GetEstadoReservaDto>()
                .ReverseMap();

            CreateMap<Entity.EstadoReserva, ListEstadoReservaDto>()
                .ReverseMap();

            CreateMap<Entity.EstadoReserva, SelectComboEstadoReservaDto>()
                .ReverseMap();

            CreateMap<Entity.EstadoReserva, SearchEstadoReservaDto>()
                .ReverseMap();
        }
    }
}
