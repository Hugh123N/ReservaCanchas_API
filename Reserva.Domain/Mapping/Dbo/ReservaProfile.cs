using AutoMapper;
using Reserva.Dto.Dbo.Reserva;

namespace Reserva.Domain.Mapping.Reserva
{
    public class ReservaProfile : Profile
    {
        public ReservaProfile()
        {
            CreateMap<Entity.Reserva, ReservaDto>()
                .ReverseMap();

            CreateMap<Entity.Reserva, CreateReservaDto>()
                .ReverseMap();

            CreateMap<Entity.Reserva, UpdateReservaDto>()
                .ReverseMap();

            CreateMap<Entity.Reserva, GetReservaDto>()
                .ForMember(dest => dest.EstadoReserva, opt => opt.MapFrom(src => src.IdEstadoReservaNavigation))
                .ReverseMap();

            CreateMap<Entity.Reserva, ListReservaDto>()
                .ReverseMap();

            CreateMap<Entity.Reserva, SelectComboReservaDto>()
                .ReverseMap();

            CreateMap<Entity.Reserva, SearchReservaDto>()
                .ReverseMap();
        }
    }
}
