using AutoMapper;
using Reserva.Dto.Cancha.EstadoUsuario;

namespace Reserva.Domain.Mapping.EstadoUsuario
{
    public class EstadoUsuarioProfile : Profile
    {
        public EstadoUsuarioProfile()
        {
            CreateMap<Entity.Models.EstadoUsuario, EstadoUsuarioDto>()
                .ReverseMap();

            CreateMap<Entity.Models.EstadoUsuario, CreateEstadoUsuarioDto>()
                .ReverseMap();

            CreateMap<Entity.Models.EstadoUsuario, UpdateEstadoUsuarioDto>()
                .ReverseMap();

            CreateMap<Entity.Models.EstadoUsuario, GetEstadoUsuarioDto>()
                .ReverseMap();

            CreateMap<Entity.Models.EstadoUsuario, ListEstadoUsuarioDto>()
                .ReverseMap();

            CreateMap<Entity.Models.EstadoUsuario, SelectComboEstadoUsuarioDto>()
                .ReverseMap();

            CreateMap<Entity.Models.EstadoUsuario, SearchEstadoUsuarioDto>()
                .ReverseMap();
        }
    }
}
