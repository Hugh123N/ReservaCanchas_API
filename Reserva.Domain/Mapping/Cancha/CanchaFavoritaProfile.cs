using AutoMapper;
using Reserva.Dto.Cancha.CanchaFavorita;

namespace Reserva.Domain.Mapping.CanchaFavorita
{
    public class CanchaFavoritaProfile : Profile
    {
        public CanchaFavoritaProfile()
        {
            CreateMap<Entity.Models.CanchaFavorita, CanchaFavoritaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.CanchaFavorita, CreateCanchaFavoritaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.CanchaFavorita, UpdateCanchaFavoritaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.CanchaFavorita, GetCanchaFavoritaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.CanchaFavorita, ListCanchaFavoritaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.CanchaFavorita, SelectComboCanchaFavoritaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.CanchaFavorita, SearchCanchaFavoritaDto>()
                .ReverseMap();
        }
    }
}
