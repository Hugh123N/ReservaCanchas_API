using AutoMapper;
using Reserva.Dto.Cancha.EstadoProveedor;

namespace Reserva.Domain.Mapping.EstadoProveedor
{
    public class EstadoProveedorProfile : Profile
    {
        public EstadoProveedorProfile()
        {
            CreateMap<Entity.Models.EstadoProveedor, EstadoProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.Models.EstadoProveedor, CreateEstadoProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.Models.EstadoProveedor, UpdateEstadoProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.Models.EstadoProveedor, GetEstadoProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.Models.EstadoProveedor, ListEstadoProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.Models.EstadoProveedor, SelectComboEstadoProveedorDto>()
                .ReverseMap();

            CreateMap<Entity.Models.EstadoProveedor, SearchEstadoProveedorDto>()
                .ReverseMap();
        }
    }
}
