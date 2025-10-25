using AutoMapper;
using Reserva.Dto.Dbo.DetallePago;

namespace Reserva.Domain.Mapping.DetallePago
{
    public class DetallePagoProfile : Profile
    {
        public DetallePagoProfile()
        {
            CreateMap<Entity.DetallePago, DetallePagoDto>()
                .ReverseMap();

            CreateMap<Entity.DetallePago, CreateDetallePagoDto>()
                .ReverseMap();

            CreateMap<Entity.DetallePago, UpdateDetallePagoDto>()
                .ReverseMap();

            CreateMap<Entity.DetallePago, GetDetallePagoDto>()
                .ReverseMap();

            CreateMap<Entity.DetallePago, ListDetallePagoDto>()
                .ReverseMap();

            CreateMap<Entity.DetallePago, SelectComboDetallePagoDto>()
                .ReverseMap();

            CreateMap<Entity.DetallePago, SearchDetallePagoDto>()
                .ReverseMap();
        }
    }
}
