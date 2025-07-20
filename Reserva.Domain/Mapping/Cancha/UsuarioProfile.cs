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

            CreateMap<Entity.Models.ApplicationUser, UsuarioDto>().ReverseMap();
            CreateMap<Entity.Models.ApplicationUser, CreateUsuarioDto>().ReverseMap();
            CreateMap<Entity.Models.ApplicationUser, UpdateUsuarioDto>().ReverseMap();
            CreateMap<Entity.Models.ApplicationUser, GetUsuarioDto>().ReverseMap();
            CreateMap<Entity.Models.ApplicationUser, ListUsuarioDto>().ReverseMap();
            CreateMap<Entity.Models.ApplicationUser, SearchUsuarioDto>().ReverseMap();
            CreateMap<Entity.Models.ApplicationUser, CreateUsuarioProveedorDto>().ReverseMap();
        }
    }
}
