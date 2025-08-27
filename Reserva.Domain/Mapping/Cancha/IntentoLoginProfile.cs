using AutoMapper;
using Reserva.Dto.Cancha.IntentoLogin;

namespace Reserva.Domain.Mapping.IntentoLogin
{
    public class IntentoLoginProfile : Profile
    {
        public IntentoLoginProfile()
        {
            CreateMap<Entity.IntentoLogin, IntentoLoginDto>()
                .ReverseMap();

            CreateMap<Entity.IntentoLogin, CreateIntentoLoginDto>()
                .ReverseMap();

            CreateMap<Entity.IntentoLogin, UpdateIntentoLoginDto>()
                .ReverseMap();

            CreateMap<Entity.IntentoLogin, GetIntentoLoginDto>()
                .ReverseMap();

            CreateMap<Entity.IntentoLogin, ListIntentoLoginDto>()
                .ReverseMap();

            CreateMap<Entity.IntentoLogin, SearchIntentoLoginDto>()
                .ReverseMap();
        }
    }
}
