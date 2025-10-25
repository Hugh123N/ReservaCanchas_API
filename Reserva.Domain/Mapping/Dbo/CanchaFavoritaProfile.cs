using AutoMapper;
using Reserva.Dto.Dbo.CanchaFavorita;

namespace Reserva.Domain.Mapping.CanchaFavorita
{
    public class CanchaFavoritaProfile : Profile
    {
        public CanchaFavoritaProfile()
        {
            CreateMap<Entity.CanchaFavorita, CanchaFavoritaDto>()
                .ReverseMap();

            CreateMap<Entity.CanchaFavorita, CreateCanchaFavoritaDto>()
                .ReverseMap();

            CreateMap<Entity.CanchaFavorita, UpdateCanchaFavoritaDto>()
                .ReverseMap();

            CreateMap<Entity.CanchaFavorita, GetCanchaFavoritaDto>()
                .ReverseMap();

            CreateMap<Entity.CanchaFavorita, ListCanchaFavoritaDto>()
                .ReverseMap();

            CreateMap<Entity.CanchaFavorita, SelectComboCanchaFavoritaDto>()
                .ReverseMap();

            CreateMap<Entity.CanchaFavorita, SearchCanchaFavoritaDto>()
                .ReverseMap();
        }
    }
}
