using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.EstadoCancha;

namespace Reserva.Domain.Commands.Dbo.EstadoCancha
{
    public class UpdateEstadoCanchaCommand : CommandBase<GetEstadoCanchaDto>
    {
        public UpdateEstadoCanchaCommand(UpdateEstadoCanchaDto updateDto) => UpdateDto = updateDto;
        public UpdateEstadoCanchaDto UpdateDto { get; set; }
    }
}
