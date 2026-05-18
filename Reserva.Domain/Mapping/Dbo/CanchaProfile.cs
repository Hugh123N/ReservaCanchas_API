using AutoMapper;
using Reserva.Dto.Dbo.Cancha;

namespace Reserva.Domain.Mapping.Cancha
{
    public class CanchaProfile : Profile
    {
        public CanchaProfile()
        {
            CreateMap<Entity.Cancha, CanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Cancha, CreateCanchaDto>()
                .ForMember(x => x.HorarioCanchas, opt => opt.MapFrom(x => x.HorarioCancha))
                .ReverseMap();

            CreateMap<Entity.Cancha, UpdateCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Cancha, GetCanchaDto>()
                .ForMember(x => x.ImagenesCancha, opt => opt.MapFrom(x => x.ImagenCancha))
                .ForMember(x => x.EstadoCancha, opt => opt.MapFrom(x => x.IdEstadoCanchaNavigation))
                .ForMember(x => x.Faboritos, opt => opt.MapFrom(x => x.CanchaFavorita))
                .ForMember(x => x.Ubigeo, opt => opt.MapFrom(x => x.CodigoUbigeoNavigation))
                .ForMember(x => x.TipoSuperficie, opt => opt.MapFrom(x => x.IdTipoSuperficieNavigation))
                .ReverseMap();

            CreateMap<Entity.Cancha, ListCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Cancha, SelectComboCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Cancha, SearchCanchaDto>()
                .ForMember(x => x.UrlImagen, opt => opt.MapFrom(x => x.ImagenCancha.Select(x => x.UrlImagen).First()))
                .ForMember(x => x.EstadoCancha, opt => opt.MapFrom(x => x.IdEstadoCanchaNavigation))
                .ForMember(x => x.Faboritos, opt => opt.MapFrom(x => x.CanchaFavorita))
                .ForMember(x => x.Ubigeo, opt => opt.MapFrom(x => x.CodigoUbigeoNavigation))
                .ReverseMap();
        }
    }
}
