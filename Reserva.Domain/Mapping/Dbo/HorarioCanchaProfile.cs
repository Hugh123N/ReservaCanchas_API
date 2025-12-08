using AutoMapper;
using Reserva.Dto.Dbo.HorarioCancha;

namespace Reserva.Domain.Mapping.HorarioCancha
{
    public class HorarioCanchaProfile : Profile
    {
        public HorarioCanchaProfile()
        {
            CreateMap<Entity.HorarioCancha, HorarioCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.HorarioCancha, CreateHorarioCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.HorarioCancha, UpdateHorarioCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.HorarioCancha, GetHorarioCanchaDto>()
                .ForMember(x => x.DiaSemana, opt => opt.MapFrom(x => x.IdDiaSemanaNavigation))
                .ForMember(x => x.HoraInicio, opt => opt.MapFrom(x => x.IdHoraInicioNavigation))
                .ReverseMap();

            CreateMap<Entity.HorarioCancha, ListHorarioCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.HorarioCancha, SelectComboHorarioCanchaDto>()
                .ReverseMap();

            CreateMap<Entity.HorarioCancha, SearchHorarioCanchaDto>()
                .ReverseMap();
        }
    }
}
