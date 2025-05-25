using AutoMapper;
using Reserva.Dto.Cancha.TipoProveedor;

namespace Reserva.Domain.Mapping.TipoProveedor
{
    public class TipoProveedorProfile : Profile
    {
        public TipoProveedorProfile()
        {
            CreateMap<Entity.Models.TipoProveedor, TipoProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.Models.TipoProveedor, CreateTipoProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.Models.TipoProveedor, UpdateTipoProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.Models.TipoProveedor, GetTipoProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.Models.TipoProveedor, ListTipoProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.Models.TipoProveedor, SelectComboTipoProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.Models.TipoProveedor, SearchTipoProveedorDto>()
                .ReverseMap();
        }
    }
}
