using System.Collections.Generic;
using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Notificacion;

namespace Reserva.Domain.Commands.Dbo.Notificacion
{
    public class CreateNotificacionesMassiveCommand : CommandBase
    {
        public List<CreateNotificacionDto> Notificaciones { get; set; } = new();

        public CreateNotificacionesMassiveCommand(List<CreateNotificacionDto> notificaciones)
        {
            Notificaciones = notificaciones ?? new List<CreateNotificacionDto>();
        }
    }
}
