using AutoMapper;
using Reserva.Dto.Dbo.Proveedor;
using Reserva.Dto.Dbo.Usuario;

namespace Reserva.Domain.Mapping.Proveedor
{
    public class ProveedorProfile : Profile
    {
        public ProveedorProfile()
        {
            CreateMap<Entity.Proveedor, ProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.Proveedor, CreateProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.Proveedor, UpdateProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.Proveedor, GetProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.Proveedor, ListProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.Proveedor, SelectComboProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.Proveedor, SearchProveedorDto>()
                .ReverseMap();
            CreateMap<Entity.Proveedor, CreateUsuarioProveedorDto>()
                .ReverseMap();
            CreateMap<Entity.Proveedor, UpgradeToProveedorDto>()
                .ReverseMap();
        }
    }
}
