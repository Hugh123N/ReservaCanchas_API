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
                .ForMember(x => x.Disponibilidades, opt => opt.MapFrom(x => x.Disponibilidad))
                .ForMember(x => x.Imagenes, opt => opt.MapFrom(x => x.ImagenCancha))
                .ReverseMap();

            CreateMap<Entity.Cancha, UpdateCanchaDto>()
                .ForMember(x => x.Disponibilidades, opt => opt.MapFrom(x => x.Disponibilidad))
                .ForMember(x => x.Imagenes, opt => opt.MapFrom(x => x.ImagenCancha))
                .ReverseMap();

            CreateMap<Entity.Cancha, GetCanchaDto>()
                .ForMember(x => x.TipoCancha, opt => opt.MapFrom(x => x.IdTipoCanchaNavigation))
                .ForMember(x => x.ImagenesCancha, opt => opt.MapFrom(x => x.ImagenCancha))
                .ForMember(x => x.EstadoCancha, opt => opt.MapFrom(x => x.IdEstadoCanchaNavigation))
                .ForMember(x => x.Faboritos, opt => opt.MapFrom(x => x.CanchaFavorita))
                .ForMember(x => x.Ubigeo, opt => opt.MapFrom(x => x.CodigoUbigeoNavigation))
                .ReverseMap();

            CreateMap<Entity.Cancha, ListCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Cancha, SelectComboCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Cancha, SearchCanchaDto>()
                .ForMember(x => x.TipoCancha, opt => opt.MapFrom(x => x.IdTipoCanchaNavigation))
                .ForMember(x => x.ImagenesCancha, opt => opt.MapFrom(x => x.ImagenCancha))
                .ForMember(x => x.EstadoCancha, opt => opt.MapFrom(x => x.IdEstadoCanchaNavigation))
                .ForMember(x => x.Faboritos, opt => opt.MapFrom(x => x.CanchaFavorita))
                .ForMember(x => x.Ubigeo, opt => opt.MapFrom(x => x.CodigoUbigeoNavigation))
                .ReverseMap();
        }
    }
}
