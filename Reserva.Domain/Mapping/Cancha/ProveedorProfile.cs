using AutoMapper;
using Reserva.Dto.Cancha.Proveedor;

namespace Reserva.Domain.Mapping.Proveedor
{
    public class ProveedorProfile : Profile
    {
        public ProveedorProfile()
        {
            CreateMap<Entity.Models.Proveedor, ProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Proveedor, CreateProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Proveedor, UpdateProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Proveedor, GetProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Proveedor, ListProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Proveedor, SelectComboProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Proveedor, SearchProveedorDto>()
                .ReverseMap();
        }
    }
}
