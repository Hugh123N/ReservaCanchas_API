using AutoMapper;
using Reserva.Dto.Cancha.MetodoPago;

namespace Reserva.Domain.Mapping.MetodoPago
{
    public class MetodoPagoProfile : Profile
    {
        public MetodoPagoProfile()
        {
            CreateMap<Entity.MetodoPago, MetodoPagoDto>()
                .ReverseMap();

            CreateMap<Entity.MetodoPago, CreateMetodoPagoDto>()
                .ReverseMap();

            CreateMap<Entity.MetodoPago, UpdateMetodoPagoDto>()
                .ReverseMap();

            CreateMap<Entity.MetodoPago, GetMetodoPagoDto>()
                .ReverseMap();

            CreateMap<Entity.MetodoPago, ListMetodoPagoDto>()
                .ReverseMap();

            CreateMap<Entity.MetodoPago, SelectComboMetodoPagoDto>()
                .ReverseMap();

            CreateMap<Entity.MetodoPago, SearchMetodoPagoDto>()
                .ReverseMap();
        }
    }
}
