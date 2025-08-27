using AutoMapper;
using Reserva.Dto.Cancha.EstadoUsuario;

namespace Reserva.Domain.Mapping.EstadoUsuario
{
    public class EstadoUsuarioProfile : Profile
    {
        public EstadoUsuarioProfile()
        {
            CreateMap<Entity.EstadoUsuario, EstadoUsuarioDto>()
                .ReverseMap();

            CreateMap<Entity.EstadoUsuario, CreateEstadoUsuarioDto>()
                .ReverseMap();

            CreateMap<Entity.EstadoUsuario, UpdateEstadoUsuarioDto>()
                .ReverseMap();

            CreateMap<Entity.EstadoUsuario, GetEstadoUsuarioDto>()
                .ReverseMap();

            CreateMap<Entity.EstadoUsuario, ListEstadoUsuarioDto>()
                .ReverseMap();

            CreateMap<Entity.EstadoUsuario, SelectComboEstadoUsuarioDto>()
                .ReverseMap();

            CreateMap<Entity.EstadoUsuario, SearchEstadoUsuarioDto>()
                .ReverseMap();
        }
    }
}
