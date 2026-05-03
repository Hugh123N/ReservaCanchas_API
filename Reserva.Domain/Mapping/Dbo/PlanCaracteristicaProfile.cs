using AutoMapper;
using Reserva.Dto.Dbo.ProveedorPlan;
using Reserva.Entity;

namespace Reserva.Domain.Mapping.Dbo
{
    public class PlanCaracteristicaProfile : Profile
    {
        public PlanCaracteristicaProfile()
        {
            CreateMap<Entity.PlanCaracteristica, PlanCaracteristicaDto>()
                .ReverseMap();
        }
    }
}
