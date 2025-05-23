using AutoMapper;
using Reserva.Dto.Cancha.TipoCancha;

namespace Reserva.Domain.Mapping.TipoCancha
{
    public class TipoCanchaProfile : Profile
    {
        public TipoCanchaProfile()
        {
            CreateMap<Entity.Models.TipoCancha, TipoCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.TipoCancha, CreateTipoCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.TipoCancha, UpdateTipoCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.TipoCancha, GetTipoCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.TipoCancha, ListTipoCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.TipoCancha, SelectComboTipoCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.TipoCancha, SearchTipoCanchaDto>()
                .ReverseMap();
        }
    }
}
