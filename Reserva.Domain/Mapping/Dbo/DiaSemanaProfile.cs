using AutoMapper;
using Reserva.Dto.Dbo.DiaSemana;

namespace Reserva.Domain.Mapping.DiaSemana
{
    public class DiaSemanaProfile : Profile
    {
        public DiaSemanaProfile()
        {
            CreateMap<Entity.DiaSemana, DiaSemanaDto>()
                .ReverseMap();

            CreateMap<Entity.DiaSemana, CreateDiaSemanaDto>()
                .ReverseMap();

            CreateMap<Entity.DiaSemana, UpdateDiaSemanaDto>()
                .ReverseMap();

            CreateMap<Entity.DiaSemana, GetDiaSemanaDto>()
                .ReverseMap();

            CreateMap<Entity.DiaSemana, ListDiaSemanaDto>()
                .ReverseMap();

            CreateMap<Entity.DiaSemana, SelectComboDiaSemanaDto>()
                .ReverseMap();

            CreateMap<Entity.DiaSemana, SearchDiaSemanaDto>()
                .ReverseMap();
            CreateMap<Entity.DiaSemana, SelectDiaSemanaDto>()
                .ReverseMap();
        }
    }
}
