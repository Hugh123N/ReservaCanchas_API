using AutoMapper;
using Reserva.Dto.Cancha.Departamento;

namespace Reserva.Domain.Mapping.Departamento
{
    public class DepartamentoProfile : Profile
    {
        public DepartamentoProfile()
        {
            CreateMap<Entity.Models.Departamento, DepartamentoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Departamento, CreateDepartamentoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Departamento, UpdateDepartamentoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Departamento, GetDepartamentoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Departamento, ListDepartamentoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Departamento, SelectComboDepartamentoDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Departamento, SearchDepartamentoDto>()
                .ReverseMap();
        }
    }
}
