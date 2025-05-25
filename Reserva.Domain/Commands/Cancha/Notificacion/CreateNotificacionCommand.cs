using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Notificacion;

namespace Reserva.Domain.Commands.Cancha.Notificacion
{
    public class CreateNotificacionCommand : CommandBase<GetNotificacionDto>
    {
        public CreateNotificacionCommand(CreateNotificacionDto createDto) => CreateDto = createDto;
        public CreateNotificacionDto CreateDto { get; set; }
    }
}
