using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;
using Reserva.Entity;

namespace Reserva.Domain.Queries.Dbo.Notificacion
{
    public class NotificacionExistsQueryHandler : QueryHandlerBase<NotificacionExistsQuery, bool>
    {
        private readonly IRepository<Entity.Notificacion> _notificacionRepository;

        public NotificacionExistsQueryHandler(
            IRepository<Entity.Notificacion> notificacionRepository
        ) : base()
        {
            _notificacionRepository = notificacionRepository;
        }

        protected override async Task<ResponseDto<bool>> HandleQuery(NotificacionExistsQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<bool>();

            var notificacion = await _notificacionRepository.GetByAsNoTrackingAsync(x => x.Activo && x.Modulo == request.Modulo && x.Tipo == request.Tipo
            && x.EntidadTipo == request.EntidadTipo && x.EntidadId == request.EntidadId);

            var exists = notificacion != null;
            
            response.UpdateData(exists);

            return await Task.FromResult(response);
        }
    }
}
