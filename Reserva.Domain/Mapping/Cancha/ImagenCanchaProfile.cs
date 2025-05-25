using AutoMapper;
using Reserva.Dto.Cancha.ImagenCancha;

namespace Reserva.Domain.Mapping.ImagenCancha
{
    public class ImagenCanchaProfile : Profile
    {
        public ImagenCanchaProfile()
        {
            CreateMap<Entity.Models.ImagenCancha, ImagenCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.ImagenCancha, CreateImagenCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.ImagenCancha, UpdateImagenCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.ImagenCancha, GetImagenCanchaDto>()
                //.ForMember(x => x.Cancha, opt => opt.MapFrom(x => x.IdCanchaNavigation))
                .ReverseMap();

            CreateMap<Entity.Models.ImagenCancha, ListImagenCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.ImagenCancha, SelectComboImagenCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.ImagenCancha, SearchImagenCanchaDto>()
                .ReverseMap();
        }
    }
}
