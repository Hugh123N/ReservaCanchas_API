using AutoMapper;
using Reserva.Dto.Cancha.Cancha;

namespace Reserva.Domain.Mapping.Cancha
{
    public class CanchaProfile : Profile
    {
        public CanchaProfile()
        {
            CreateMap<Entity.Models.Cancha, CanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Cancha, CreateCanchaDto>()
                .ForMember(x => x.Disponibilidades, opt => opt.MapFrom(x => x.Disponibilidads))
                .ForMember(x => x.Imagenes, opt => opt.MapFrom(x => x.ImagenCanchas))
                .ReverseMap();

            CreateMap<Entity.Models.Cancha, UpdateCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Cancha, GetCanchaDto>()
                .ForMember(x => x.TipoCancha, opt => opt.MapFrom(x => x.IdTipoCanchaNavigation))
                .ForMember(x => x.ImagenesCancha, opt => opt.MapFrom(x => x.ImagenCanchas))
                .ForMember(x => x.EstadoCancha, opt => opt.MapFrom(x => x.IdEstadoCanchaNavigation))
                .ForMember(x => x.Faboritos, opt => opt.MapFrom(x => x.CanchaFavorita))
                .ForMember(x => x.Ubigeo, opt => opt.MapFrom(x => x.CodigoUbigeoNavigation))
                .ReverseMap();

            CreateMap<Entity.Models.Cancha, ListCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Cancha, SelectComboCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Cancha, SearchCanchaDto>()
                .ForMember(x => x.TipoCancha, opt => opt.MapFrom(x => x.IdTipoCanchaNavigation))
                .ForMember(x => x.ImagenesCancha, opt => opt.MapFrom(x => x.ImagenCanchas))
                .ForMember(x => x.EstadoCancha, opt => opt.MapFrom(x => x.IdEstadoCanchaNavigation))
                .ForMember(x => x.Faboritos, opt => opt.MapFrom(x => x.CanchaFavorita))
                .ForMember(x => x.Ubigeo, opt => opt.MapFrom(x => x.CodigoUbigeoNavigation))
                .ReverseMap();
        }
    }
}
