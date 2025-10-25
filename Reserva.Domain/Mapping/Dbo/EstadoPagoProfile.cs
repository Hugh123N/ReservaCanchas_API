using AutoMapper;
using Reserva.Dto.Dbo.EstadoPago;

namespace Reserva.Domain.Mapping.EstadoPago
{
    public class EstadoPagoProfile : Profile
    {
        public EstadoPagoProfile()
        {
            CreateMap<Entity.EstadoPago, EstadoPagoDto>()
                .ReverseMap();

            CreateMap<Entity.EstadoPago, CreateEstadoPagoDto>()
                .ReverseMap();

            CreateMap<Entity.EstadoPago, UpdateEstadoPagoDto>()
                .ReverseMap();

            CreateMap<Entity.EstadoPago, GetEstadoPagoDto>()
                .ReverseMap();

            CreateMap<Entity.EstadoPago, ListEstadoPagoDto>()
                .ReverseMap();

            CreateMap<Entity.EstadoPago, SelectComboEstadoPagoDto>()
                .ReverseMap();

            CreateMap<Entity.EstadoPago, SearchEstadoPagoDto>()
                .ReverseMap();
        }
    }
}
