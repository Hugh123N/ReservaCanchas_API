using AutoMapper;
using Reserva.Dto.Dbo.TipoSuperficie;

namespace Reserva.Domain.Mapping.TipoSuperficie
{
    public class TipoSuperficieProfile : Profile
    {
        public TipoSuperficieProfile()
        {
            CreateMap<Entity.TipoSuperficie, TipoSuperficieDto>()
                .ReverseMap();

            CreateMap<Entity.TipoSuperficie, CreateTipoSuperficieDto>()
                .ReverseMap();

            CreateMap<Entity.TipoSuperficie, UpdateTipoSuperficieDto>()
                .ReverseMap();

            CreateMap<Entity.TipoSuperficie, GetTipoSuperficieDto>()
                .ReverseMap();

            CreateMap<Entity.TipoSuperficie, SelectComboTipoSuperficieDto>()
                .ReverseMap();

        }
    }
}
