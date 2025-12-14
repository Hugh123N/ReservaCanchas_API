using AutoMapper;
using Reserva.Dto.Dbo.Operador;

namespace Reserva.Domain.Mapping.Operador
{
    public class OperadorProfile : Profile
    {
        public OperadorProfile()
        {
            CreateMap<Entity.Operador, OperadorDto>()
                .ReverseMap();

            CreateMap<Entity.Operador, CreateOperadorDto>()
                .ReverseMap();

            CreateMap<Entity.Operador, UpdateOperadorDto>()
                .ReverseMap();

            CreateMap<Entity.Operador, GetOperadorDto>()
                .ReverseMap();

            CreateMap<Entity.Operador, ListOperadorDto>()
                .ReverseMap();

            CreateMap<Entity.Operador, SearchOperadorDto>()
                .ReverseMap();
        }
    }
}
