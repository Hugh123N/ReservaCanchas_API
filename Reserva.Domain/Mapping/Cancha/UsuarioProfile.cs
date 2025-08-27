using AutoMapper;
using Reserva.Dto.Cancha.Usuario;

namespace Reserva.Domain.Mapping.Usuario
{
    public class UsuarioProfile : Profile
    {
        public UsuarioProfile()
        {
            CreateMap<Entity.AspNetUsers, UsuarioDto>()
                .ReverseMap();
            CreateMap<Entity.AspNetUsers, CreateUsuarioDto>()
                .ReverseMap();
            CreateMap<Entity.AspNetUsers, UpdateUsuarioDto>()
                .ReverseMap();
            CreateMap<Entity.AspNetUsers, GetUsuarioDto>()
                .ReverseMap();
            CreateMap<Entity.AspNetUsers, ListUsuarioDto>()
                .ReverseMap();
            CreateMap<Entity.AspNetUsers, SelectComboUsuarioDto>()
                .ReverseMap();
            CreateMap<Entity.AspNetUsers, SearchUsuarioDto>()
                .ReverseMap();

            CreateMap<Entity.ApplicationUser, UsuarioDto>().ReverseMap();
            CreateMap<Entity.ApplicationUser, CreateUsuarioDto>().ReverseMap();
            CreateMap<Entity.ApplicationUser, UpdateUsuarioDto>().ReverseMap();
            CreateMap<Entity.ApplicationUser, GetUsuarioDto>().ReverseMap();
            CreateMap<Entity.ApplicationUser, ListUsuarioDto>().ReverseMap();
            CreateMap<Entity.ApplicationUser, SearchUsuarioDto>().ReverseMap();
            CreateMap<Entity.ApplicationUser, CreateUsuarioProveedorDto>().ReverseMap();
        }
    }
}
