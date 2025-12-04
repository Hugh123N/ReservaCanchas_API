using AutoMapper;
using Reserva.Dto.Dbo.Servicio;

namespace Reserva.Domain.Mapping.Servicio
{
    public class ServicioProfile : Profile
    {
        public ServicioProfile()
        {
            CreateMap<Entity.Servicio, ServicioDto>()
                .ReverseMap();

            CreateMap<Entity.Servicio, CreateServicioDto>()
                .ReverseMap();

            CreateMap<Entity.Servicio, UpdateServicioDto>()
                .ReverseMap();

            CreateMap<Entity.Servicio, GetServicioDto>()
                .ReverseMap();

            CreateMap<Entity.Servicio, SelectComboServicioDto>()
                .ReverseMap();
        }
    }
}
