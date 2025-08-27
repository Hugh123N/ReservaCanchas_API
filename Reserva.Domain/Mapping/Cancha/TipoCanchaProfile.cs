using AutoMapper;
using Reserva.Dto.Cancha.TipoCancha;

namespace Reserva.Domain.Mapping.TipoCancha
{
    public class TipoCanchaProfile : Profile
    {
        public TipoCanchaProfile()
        {
            CreateMap<Entity.TipoCancha, TipoCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.TipoCancha, CreateTipoCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.TipoCancha, UpdateTipoCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.TipoCancha, GetTipoCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.TipoCancha, ListTipoCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.TipoCancha, SelectComboTipoCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.TipoCancha, SearchTipoCanchaDto>()
                .ReverseMap();
        }
    }
}
