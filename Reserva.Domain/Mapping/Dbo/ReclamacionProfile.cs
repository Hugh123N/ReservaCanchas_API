using AutoMapper;
using Reserva.Dto.Dbo.Reclamacion;

namespace Reserva.Domain.Mapping.Reclamacion
{
    public class ReclamacionProfile : Profile
    {
        public ReclamacionProfile()
        {
            CreateMap<CreateReclamacionDto, Entity.Reclamacion>(MemberList.None);
            CreateMap<Entity.Reclamacion, GetReclamacionDto>(MemberList.None);
            CreateMap<Entity.Reclamacion, SearchReclamacionDto>();
        }
    }
}
