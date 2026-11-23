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
                .ReverseMap()
                .ForMember(c => c.IdEstadoCancha, o => o.Ignore())
                .ForMember(c => c.IdProveedor, o => o.Ignore());

            CreateMap<Entity.Cancha, GetCanchaDto>()
                .ForMember(x => x.Version, opt => opt.MapFrom(x => x.UpdateDate ?? x.CreateDate))
                .ForMember(x => x.ImagenesCancha, opt => opt.MapFrom(x => x.ImagenCancha.Where(i => i.Activo)))
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
                .ForMember(x => x.Version, opt => opt.MapFrom(x => x.UpdateDate ?? x.CreateDate))
                .ForMember(x => x.UrlImagen, opt => opt.MapFrom(x => x.ImagenCancha
                    .Where(i => i.Activo && i.EsPrincipal).OrderBy(i => i.IdImagenCancha)
                    .Select(i => i.UrlImagen).FirstOrDefault()))
                .ForMember(x => x.EstadoCancha, opt => opt.MapFrom(x => x.IdEstadoCanchaNavigation))
                .ForMember(x => x.Faboritos, opt => opt.MapFrom(x => x.CanchaFavorita))
                .ForMember(x => x.Ubigeo, opt => opt.MapFrom(x => x.CodigoUbigeoNavigation))
                .ReverseMap();
        }
    }
}
