using AutoMapper;
using Reserva.Dto.Cancha.Pago;

namespace Reserva.Domain.Mapping.Pago
{
    public class PagoProfile : Profile
    {
        public PagoProfile()
        {
            CreateMap<Entity.Models.Pago, PagoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Pago, CreatePagoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Pago, UpdatePagoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Pago, GetPagoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Pago, ListPagoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Pago, SelectComboPagoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Pago, SearchPagoDto>()
                .ReverseMap();
        }
    }
}
