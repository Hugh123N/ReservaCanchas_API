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
