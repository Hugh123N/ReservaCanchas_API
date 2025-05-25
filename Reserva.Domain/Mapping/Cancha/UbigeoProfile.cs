using AutoMapper;
using Reserva.Dto.Cancha.Ubigeo;

namespace Reserva.Domain.Mapping.Cancha
{
    public class UbigeoProfile : Profile
    {
        public UbigeoProfile()
        {
            CreateMap<Entity.Models.Ubigeo, UbigeoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Ubigeo, CreateUbigeoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Ubigeo, UpdateUbigeoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Ubigeo, GetUbigeoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Ubigeo, ListUbigeoDto>()
                .ReverseMap();

        }
    }
}
