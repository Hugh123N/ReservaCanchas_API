using AutoMapper;
using Reserva.Dto.Cancha.EstadoReserva;

namespace Reserva.Domain.Mapping.EstadoReserva
{
    public class EstadoReservaProfile : Profile
    {
        public EstadoReservaProfile()
        {
            CreateMap<Entity.Models.EstadoReserva, EstadoReservaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.EstadoReserva, CreateEstadoReservaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.EstadoReserva, UpdateEstadoReservaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.EstadoReserva, GetEstadoReservaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.EstadoReserva, ListEstadoReservaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.EstadoReserva, SelectComboEstadoReservaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.EstadoReserva, SearchEstadoReservaDto>()
                .ReverseMap();
        }
    }
}
