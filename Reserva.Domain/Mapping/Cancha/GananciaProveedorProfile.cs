using AutoMapper;
using Reserva.Dto.Cancha.GananciaProveedor;

namespace Reserva.Domain.Mapping.GananciaProveedor
{
    public class GananciaProveedorProfile : Profile
    {
        public GananciaProveedorProfile()
        {
            CreateMap<Entity.Models.GananciaProveedor, GananciaProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.Models.GananciaProveedor, CreateGananciaProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.Models.GananciaProveedor, UpdateGananciaProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.Models.GananciaProveedor, GetGananciaProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.Models.GananciaProveedor, ListGananciaProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.Models.GananciaProveedor, SelectComboGananciaProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.Models.GananciaProveedor, SearchGananciaProveedorDto>()
                .ReverseMap();
        }
    }
}
