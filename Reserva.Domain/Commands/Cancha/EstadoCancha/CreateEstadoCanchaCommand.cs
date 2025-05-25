using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.EstadoCancha;

namespace Reserva.Domain.Commands.Cancha.EstadoCancha
{
    public class CreateEstadoCanchaCommand : CommandBase<GetEstadoCanchaDto>
    {
        public CreateEstadoCanchaCommand(CreateEstadoCanchaDto createDto) => CreateDto = createDto;
        public CreateEstadoCanchaDto CreateDto { get; set; }
    }
}
