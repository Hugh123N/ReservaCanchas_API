using AutoMapper;
using Reserva.Dto.Cancha.Provincia;

namespace Reserva.Domain.Mapping.Provincia
{
    public class ProvinciaProfile : Profile
    {
        public ProvinciaProfile()
        {
            CreateMap<Entity.Models.Provincia, ProvinciaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Provincia, CreateProvinciaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Provincia, UpdateProvinciaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Provincia, GetProvinciaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Provincia, ListProvinciaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Provincia, SelectComboProvinciaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Provincia, SearchProvinciaDto>()
                .ReverseMap();
        }
    }
}
