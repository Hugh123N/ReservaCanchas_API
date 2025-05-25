using AutoMapper;
using Reserva.Dto.Cancha.Rol;

namespace Reserva.Domain.Mapping.Rol
{
    public class RolProfile : Profile
    {
        public RolProfile()
        {
            CreateMap<Entity.Models.Rol, RolDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Rol, CreateRolDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Rol, UpdateRolDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Rol, GetRolDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Rol, ListRolDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Rol, SelectComboRolDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Rol, SearchRolDto>()
                .ReverseMap();
        }
    }
}
