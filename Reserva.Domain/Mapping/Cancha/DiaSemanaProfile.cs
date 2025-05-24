using AutoMapper;
using Reserva.Dto.Cancha.DiaSemana;

namespace Reserva.Domain.Mapping.DiaSemana
{
    public class DiaSemanaProfile : Profile
    {
        public DiaSemanaProfile()
        {
            CreateMap<Entity.Models.DiaSemana, DiaSemanaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.DiaSemana, CreateDiaSemanaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.DiaSemana, UpdateDiaSemanaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.DiaSemana, GetDiaSemanaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.DiaSemana, ListDiaSemanaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.DiaSemana, SelectComboDiaSemanaDto>()
                .ReverseMap();

            CreateMap<Entity.Models.DiaSemana, SearchDiaSemanaDto>()
                .ReverseMap();
            CreateMap<Entity.Models.DiaSemana, SelectDiaSemanaDto>()
                .ReverseMap();
        }
    }
}
