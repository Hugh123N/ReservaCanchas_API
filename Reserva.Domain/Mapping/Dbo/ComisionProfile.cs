using AutoMapper;
using Reserva.Dto.Dbo.Comision;

namespace Reserva.Domain.Mapping.Comision
{
    public class ComisionProfile : Profile
    {
        public ComisionProfile()
        {
            CreateMap<Entity.Comision, ComisionDto>()
                .ReverseMap();

            CreateMap<Entity.Comision, CreateComisionDto>()
                .ReverseMap();

            CreateMap<Entity.Comision, UpdateComisionDto>()
                .ReverseMap();

            CreateMap<Entity.Comision, GetComisionDto>()
                .ReverseMap();

            CreateMap<Entity.Comision, ListComisionDto>()
                .ReverseMap();

            CreateMap<Entity.Comision, SelectComboComisionDto>()
                .ReverseMap();

            CreateMap<Entity.Comision, SearchComisionDto>()
                .ReverseMap();
        }
    }
}
