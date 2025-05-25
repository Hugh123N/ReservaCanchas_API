using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.EstadoCancha;

namespace Reserva.Domain.Commands.Cancha.EstadoCancha
{
    public class UpdateEstadoCanchaCommand : CommandBase<GetEstadoCanchaDto>
    {
        public UpdateEstadoCanchaCommand(UpdateEstadoCanchaDto updateDto) => UpdateDto = updateDto;
        public UpdateEstadoCanchaDto UpdateDto { get; set; }
    }
}
