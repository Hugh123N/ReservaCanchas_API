using AutoMapper;
using Reserva.Dto.Dbo.ComprobantePagoPlan;

namespace Reserva.Domain.Mapping.ComprobantePagoPlan
{
    public class ComprobantePagoPlanProfile : Profile
    {
        public ComprobantePagoPlanProfile()
        {
            CreateMap<Entity.ComprobantePagoPlan, ComprobantePagoPlanDto>()
                .ReverseMap();

            CreateMap<Entity.ComprobantePagoPlan, CreateComprobantePagoPlanDto>()
                .ReverseMap();

            CreateMap<Entity.ComprobantePagoPlan, UpdateComprobantePagoPlanDto>()
                .ReverseMap();

            CreateMap<Entity.ComprobantePagoPlan, GetComprobantePagoPlanDto>()
                .ReverseMap();

            CreateMap<Entity.ComprobantePagoPlan, ListComprobantePagoPlanDto>()
                .ReverseMap();

            CreateMap<Entity.ComprobantePagoPlan, SearchComprobantePagoPlanDto>()
                .ReverseMap();
        }
    }
}
