using AutoMapper;
using Reserva.Dto.Cancha.MetodoPago;

namespace Reserva.Domain.Mapping.MetodoPago
{
    public class MetodoPagoProfile : Profile
    {
        public MetodoPagoProfile()
        {
            CreateMap<Entity.Models.MetodoPago, MetodoPagoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.MetodoPago, CreateMetodoPagoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.MetodoPago, UpdateMetodoPagoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.MetodoPago, GetMetodoPagoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.MetodoPago, ListMetodoPagoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.MetodoPago, SelectComboMetodoPagoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.MetodoPago, SearchMetodoPagoDto>()
                .ReverseMap();
        }
    }
}
