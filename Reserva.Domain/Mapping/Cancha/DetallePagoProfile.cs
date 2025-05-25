using AutoMapper;
using Reserva.Dto.Cancha.DetallePago;

namespace Reserva.Domain.Mapping.DetallePago
{
    public class DetallePagoProfile : Profile
    {
        public DetallePagoProfile()
        {
            CreateMap<Entity.Models.DetallePago, DetallePagoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.DetallePago, CreateDetallePagoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.DetallePago, UpdateDetallePagoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.DetallePago, GetDetallePagoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.DetallePago, ListDetallePagoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.DetallePago, SelectComboDetallePagoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.DetallePago, SearchDetallePagoDto>()
                .ReverseMap();
        }
    }
}
