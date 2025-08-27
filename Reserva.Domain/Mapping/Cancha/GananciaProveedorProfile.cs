using AutoMapper;
using Reserva.Dto.Cancha.GananciaProveedor;

namespace Reserva.Domain.Mapping.GananciaProveedor
{
    public class GananciaProveedorProfile : Profile
    {
        public GananciaProveedorProfile()
        {
            CreateMap<Entity.GananciaProveedor, GananciaProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.GananciaProveedor, CreateGananciaProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.GananciaProveedor, UpdateGananciaProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.GananciaProveedor, GetGananciaProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.GananciaProveedor, ListGananciaProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.GananciaProveedor, SelectComboGananciaProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.GananciaProveedor, SearchGananciaProveedorDto>()
                .ReverseMap();
        }
    }
}
