using AutoMapper;
using Reserva.Dto.Cancha.Usuario;

namespace Reserva.Domain.Mapping.Usuario
{
    public class UsuarioProfile : Profile
    {
        public UsuarioProfile()
        {
            CreateMap<Entity.Models.Usuario, UsuarioDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Usuario, CreateUsuarioDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Usuario, UpdateUsuarioDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Usuario, GetUsuarioDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Usuario, ListUsuarioDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Usuario, SelectComboUsuarioDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Usuario, SearchUsuarioDto>()
                .ReverseMap();
        }
    }
}
