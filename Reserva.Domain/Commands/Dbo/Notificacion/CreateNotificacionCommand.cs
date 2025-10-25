using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Notificacion;

namespace Reserva.Domain.Commands.Dbo.Notificacion
{
    public class CreateNotificacionCommand : CommandBase<GetNotificacionDto>
    {
        public CreateNotificacionCommand(CreateNotificacionDto createDto) => CreateDto = createDto;
        public CreateNotificacionDto CreateDto { get; set; }
    }
}
