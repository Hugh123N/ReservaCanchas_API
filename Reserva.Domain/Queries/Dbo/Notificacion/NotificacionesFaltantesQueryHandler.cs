using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Reserva.Domain.Queries.Base;
using Reserva.Dto.Base;
using Reserva.Repository.Abstractions.Base;

namespace Reserva.Domain.Queries.Dbo.Notificacion
{
    public class NotificacionesFaltantesQueryHandler : QueryHandlerBase<NotificacionesFaltantesQuery, List<string>>
    {
        private readonly IRepository<Entity.Notificacion> _notificacionRepository;

        public NotificacionesFaltantesQueryHandler(
            IRepository<Entity.Notificacion> notificacionRepository
        ) : base()
        {
            _notificacionRepository = notificacionRepository;
        }

        protected override async Task<ResponseDto<List<string>>> HandleQuery(NotificacionesFaltantesQuery request, CancellationToken cancellationToken)
        {
            var response = new ResponseDto<List<string>>();

            if (request.EntidadIds == null || !request.EntidadIds.Any())
            {
                response.UpdateData(new List<string>());
                return await Task.FromResult(response);
            }

            var ids = request.EntidadIds.Select(id => id.ToString()).ToList();

            var notificaciones = await _notificacionRepository.FindByAsNoTrackingAsync(x => x.Activo 
                    && x.Modulo == request.Modulo && x.Tipo == request.Tipo && x.EntidadTipo == request.EntidadTipo
                    && ids.Contains(x.EntidadId ?? string.Empty));

            var idsConNotificacion = notificaciones.Select(x => x.EntidadId ?? string.Empty).Distinct().ToList();

            // IDs que faltan (no tienen notificación)
            var idsFaltantes = new List<string>(request.EntidadIds.Where(id => !idsConNotificacion.Contains(id)));

            response.UpdateData(idsFaltantes);
            return await Task.FromResult(response);
        }
    }
}
