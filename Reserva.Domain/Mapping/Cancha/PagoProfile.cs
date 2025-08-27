using AutoMapper;
using Reserva.Dto.Cancha.Pago;

namespace Reserva.Domain.Mapping.Pago
{
    public class PagoProfile : Profile
    {
        public PagoProfile()
        {
            CreateMap<Entity.Pago, PagoDto>()
                .ReverseMap();

            CreateMap<Entity.Pago, CreatePagoDto>()
                .ReverseMap();

            CreateMap<Entity.Pago, UpdatePagoDto>()
                .ReverseMap();

            CreateMap<Entity.Pago, GetPagoDto>()
                .ReverseMap();

            CreateMap<Entity.Pago, ListPagoDto>()
                .ReverseMap();

            CreateMap<Entity.Pago, SelectComboPagoDto>()
                .ReverseMap();

            CreateMap<Entity.Pago, SearchPagoDto>()
                .ReverseMap();
        }
    }
}
