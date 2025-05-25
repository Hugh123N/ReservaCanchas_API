using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Notificacion;

namespace Reserva.Domain.Commands.Cancha.Notificacion
{
    public class UpdateNotificacionCommand : CommandBase<GetNotificacionDto>
    {
        public UpdateNotificacionCommand(UpdateNotificacionDto updateDto) => UpdateDto = updateDto;
        public UpdateNotificacionDto UpdateDto { get; set; }
    }
}
