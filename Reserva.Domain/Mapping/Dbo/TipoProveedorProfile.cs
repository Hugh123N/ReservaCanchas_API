using AutoMapper;
using Reserva.Dto.Dbo.TipoProveedor;

namespace Reserva.Domain.Mapping.TipoProveedor
{
    public class TipoProveedorProfile : Profile
    {
        public TipoProveedorProfile()
        {
            CreateMap<Entity.TipoProveedor, TipoProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.TipoProveedor, CreateTipoProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.TipoProveedor, UpdateTipoProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.TipoProveedor, GetTipoProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.TipoProveedor, ListTipoProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.TipoProveedor, SelectComboTipoProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.TipoProveedor, SearchTipoProveedorDto>()
                .ReverseMap();
        }
    }
}
