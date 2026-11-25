using AutoMapper;
using Reserva.Domain.Queries.Base;
using Reserva.Domain.Services.Reclamaciones;
using Reserva.Dto.Base;
using Reserva.Dto.Dbo.Reclamacion;
using Reserva.Repository.Abstractions.Base;
using Reserva.Repository.Security;

namespace Reserva.Domain.Queries.Dbo.Reclamacion
{
    public class GetReclamacionQueryHandler : QueryHandlerBase<GetReclamacionQuery, GetReclamacionDto>
    {
        private readonly IRepository<Entity.Reclamacion> _ReclamacionRepository;
        private readonly IUserIdentity _userIdentity;

        public GetReclamacionQueryHandler(
            IMapper mapper,
            GetReclamacionQueryValidator validator,
            IRepository<Entity.Reclamacion> ReclamacionRepository,
            IUserIdentity userIdentity
        ) : base(mapper, validator)
        {
            _ReclamacionRepository = ReclamacionRepository;
            _userIdentity = userIdentity;
        }

        protected override async Task<ResponseDto<GetReclamacionDto>> HandleQuery(GetReclamacionQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<GetReclamacionDto>();
            if (!ReclamacionReglas.EsAdmin(_userIdentity))
            {
                response.AddErrorResult("Acceso restringido al administrador.");
                return response;
            }

            var reclamacion = await _ReclamacionRepository.GetByAsNoTrackingAsync(x => x.IdReclamacion == request.Id && x.Activo);
            if (reclamacion == null)
            {
                response.AddErrorResult("Hoja no encontrada.");
                return response;
            }

            response.UpdateData(_mapper!.Map<GetReclamacionDto>(reclamacion));
            return response;
        }
    }
}
