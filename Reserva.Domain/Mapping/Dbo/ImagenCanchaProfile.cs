using AutoMapper;
using Reserva.Dto.Dbo.ImagenCancha;

namespace Reserva.Domain.Mapping.ImagenCancha
{
    public class ImagenCanchaProfile : Profile
    {
        public ImagenCanchaProfile()
        {
            CreateMap<Entity.ImagenCancha, ImagenCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.ImagenCancha, CreateImagenCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.ImagenCancha, UpdateImagenCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.ImagenCancha, GetImagenCanchaDto>()
                //.ForMember(x => x.Cancha, opt => opt.MapFrom(x => x.IdCanchaNavigation))
                .ReverseMap();

            CreateMap<Entity.ImagenCancha, ListImagenCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.ImagenCancha, SelectComboImagenCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.ImagenCancha, SearchImagenCanchaDto>()
                .ReverseMap();
        }
    }
}
