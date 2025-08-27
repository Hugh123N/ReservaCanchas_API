using AutoMapper;
using Reserva.Dto.Cancha.EstadoProveedor;

namespace Reserva.Domain.Mapping.EstadoProveedor
{
    public class EstadoProveedorProfile : Profile
    {
        public EstadoProveedorProfile()
        {
            CreateMap<Entity.EstadoProveedor, EstadoProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.EstadoProveedor, CreateEstadoProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.EstadoProveedor, UpdateEstadoProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.EstadoProveedor, GetEstadoProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.EstadoProveedor, ListEstadoProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.EstadoProveedor, SelectComboEstadoProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.EstadoProveedor, SearchEstadoProveedorDto>()
                .ReverseMap();
        }
    }
}
