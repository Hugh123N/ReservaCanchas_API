using AutoMapper;
using Reserva.Dto.Dbo.DetalleReserva;

namespace Reserva.Domain.Mapping.DetalleReserva
{
    public class DetalleReservaProfile : Profile
    {
        public DetalleReservaProfile()
        {
            CreateMap<Entity.DetalleReserva, DetalleReservaDto>()
                .ReverseMap();

            CreateMap<Entity.DetalleReserva, CreateDetalleReservaDto>()
                .ReverseMap();

            CreateMap<Entity.DetalleReserva, UpdateDetalleReservaDto>()
                .ReverseMap();

            CreateMap<Entity.DetalleReserva, GetDetalleReservaDto>()
                .ReverseMap();

            CreateMap<Entity.DetalleReserva, ListDetalleReservaDto>()
                .ReverseMap();

        }
    }
}
