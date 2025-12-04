using AutoMapper;
using Reserva.Dto.Dbo.TipoDeporte;

namespace Reserva.Domain.Mapping.TipoDeporte
{
    public class TipoDeporteProfile : Profile
    {
        public TipoDeporteProfile()
        {
            CreateMap<Entity.TipoDeporte, TipoDeporteDto>()
                .ReverseMap();

            CreateMap<Entity.TipoDeporte, CreateTipoDeporteDto>()
                .ReverseMap();

            CreateMap<Entity.TipoDeporte, UpdateTipoDeporteDto>()
                .ReverseMap();

            CreateMap<Entity.TipoDeporte, GetTipoDeporteDto>()
                .ReverseMap();

            CreateMap<Entity.TipoDeporte, SelectComboTipoDeporteDto>()
                .ReverseMap();

        }
    }
}
