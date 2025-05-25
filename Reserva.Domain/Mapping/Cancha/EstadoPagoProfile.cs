using AutoMapper;
using Reserva.Dto.Cancha.EstadoPago;

namespace Reserva.Domain.Mapping.EstadoPago
{
    public class EstadoPagoProfile : Profile
    {
        public EstadoPagoProfile()
        {
            CreateMap<Entity.Models.EstadoPago, EstadoPagoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.EstadoPago, CreateEstadoPagoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.EstadoPago, UpdateEstadoPagoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.EstadoPago, GetEstadoPagoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.EstadoPago, ListEstadoPagoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.EstadoPago, SelectComboEstadoPagoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.EstadoPago, SearchEstadoPagoDto>()
                .ReverseMap();
        }
    }
}
