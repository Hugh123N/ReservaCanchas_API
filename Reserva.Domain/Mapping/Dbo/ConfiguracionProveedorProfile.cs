using AutoMapper;
using Reserva.Dto.Dbo.ConfiguracionProveedor;

namespace Reserva.Domain.Mapping.ConfiguracionProveedor
{
    public class ConfiguracionProveedorProfile : Profile
    {
        public ConfiguracionProveedorProfile()
        {
            CreateMap<Entity.ConfiguracionProveedor, ConfiguracionProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.ConfiguracionProveedor, CreateConfiguracionProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.ConfiguracionProveedor, UpdateConfiguracionProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.ConfiguracionProveedor, GetConfiguracionProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.ConfiguracionProveedor, ListConfiguracionProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.ConfiguracionProveedor, SelectComboConfiguracionProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.ConfiguracionProveedor, SearchConfiguracionProveedorDto>()
                .ReverseMap();
        }
    }
}
