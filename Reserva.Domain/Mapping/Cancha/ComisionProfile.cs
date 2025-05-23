using AutoMapper;
using Reserva.Dto.Cancha.Comision;

namespace Reserva.Domain.Mapping.Comision
{
    public class ComisionProfile : Profile
    {
        public ComisionProfile()
        {
            CreateMap<Entity.Models.Comision, ComisionDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Comision, CreateComisionDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Comision, UpdateComisionDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Comision, GetComisionDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Comision, ListComisionDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Comision, SelectComboComisionDto>()
                .ReverseMap();

            CreateMap<Entity.Models.Comision, SearchComisionDto>()
                .ReverseMap();
        }
    }
}
