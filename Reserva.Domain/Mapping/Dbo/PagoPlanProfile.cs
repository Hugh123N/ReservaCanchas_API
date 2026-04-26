using AutoMapper;
using Reserva.Dto.Dbo.PagoPlan;

namespace Reserva.Domain.Mapping.PagoPlan
{
    public class PagoPlanProfile : Profile
    {
        public PagoPlanProfile()
        {
            CreateMap<Entity.PagoPlan, PagoPlanDto>()
                .ReverseMap();

            CreateMap<Entity.PagoPlan, CreatePagoPlanDto>()
                .ReverseMap();

            CreateMap<Entity.PagoPlan, UpdatePagoPlanDto>()
                .ReverseMap();

            CreateMap<Entity.PagoPlan, GetPagoPlanDto>()
                .ReverseMap();

            CreateMap<Entity.PagoPlan, ListPagoPlanDto>()
                .ReverseMap();

            CreateMap<Entity.PagoPlan, SearchPagoPlanDto>()
                .ReverseMap();
        }
    }
}
