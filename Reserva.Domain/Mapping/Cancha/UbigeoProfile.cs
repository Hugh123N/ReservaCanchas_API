using AutoMapper;
using Reserva.Dto.Cancha.Ubigeo;

namespace Reserva.Domain.Mapping.Cancha
{
    public class UbigeoProfile : Profile
    {
        public UbigeoProfile()
        {
            CreateMap<Entity.Ubigeo, UbigeoDto>()
                .ReverseMap();

            CreateMap<Entity.Ubigeo, CreateUbigeoDto>()
                .ReverseMap();

            CreateMap<Entity.Ubigeo, UpdateUbigeoDto>()
                .ReverseMap();

            CreateMap<Entity.Ubigeo, GetUbigeoDto>()
                .ReverseMap();

            CreateMap<Entity.Ubigeo, ListUbigeoDto>()
                .ReverseMap();

        }
    }
}
