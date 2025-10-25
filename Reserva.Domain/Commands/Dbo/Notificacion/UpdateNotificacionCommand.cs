using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Notificacion;

namespace Reserva.Domain.Commands.Dbo.Notificacion
{
    public class UpdateNotificacionCommand : CommandBase<GetNotificacionDto>
    {
        public UpdateNotificacionCommand(UpdateNotificacionDto updateDto) => UpdateDto = updateDto;
        public UpdateNotificacionDto UpdateDto { get; set; }
    }
}
