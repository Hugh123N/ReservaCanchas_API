using AutoMapper;
using Reserva.Dto.Cancha.Reserva;

namespace Reserva.Domain.Mapping.Reserva
{
    public class ReservaProfile : Profile
    {
        public ReservaProfile()
        {
            CreateMap<Entity.Models.Reserva, ReservaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Reserva, CreateReservaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Reserva, UpdateReservaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Reserva, GetReservaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Reserva, ListReservaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Reserva, SelectComboReservaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Reserva, SearchReservaDto>()
                .ReverseMap();
        }
    }
}
