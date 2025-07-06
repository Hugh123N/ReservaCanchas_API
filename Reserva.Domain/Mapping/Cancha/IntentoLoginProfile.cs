using AutoMapper;
using Reserva.Dto.Cancha.IntentoLogin;

namespace Reserva.Domain.Mapping.IntentoLogin
{
    public class IntentoLoginProfile : Profile
    {
        public IntentoLoginProfile()
        {
            CreateMap<Entity.Models.IntentoLogin, IntentoLoginDto>()
                .ReverseMap();

            CreateMap<Entity.Models.IntentoLogin, CreateIntentoLoginDto>()
                .ReverseMap();

            CreateMap<Entity.Models.IntentoLogin, UpdateIntentoLoginDto>()
                .ReverseMap();

            CreateMap<Entity.Models.IntentoLogin, GetIntentoLoginDto>()
                .ReverseMap();

            CreateMap<Entity.Models.IntentoLogin, ListIntentoLoginDto>()
                .ReverseMap();

            CreateMap<Entity.Models.IntentoLogin, SearchIntentoLoginDto>()
                .ReverseMap();
        }
    }
}
