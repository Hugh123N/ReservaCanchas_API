using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.EstadoCancha;

namespace Reserva.Domain.Commands.Dbo.EstadoCancha
{
    public class CreateEstadoCanchaCommand : CommandBase<GetEstadoCanchaDto>
    {
        public CreateEstadoCanchaCommand(CreateEstadoCanchaDto createDto) => CreateDto = createDto;
        public CreateEstadoCanchaDto CreateDto { get; set; }
    }
}
