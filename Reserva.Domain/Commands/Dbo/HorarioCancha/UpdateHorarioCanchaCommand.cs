using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.HorarioCancha;

namespace Reserva.Domain.Commands.Dbo.HorarioCancha
{
    public class UpdateHorarioCanchaCommand : CommandBase<GetHorarioCanchaDto>
    {
        public UpdateHorarioCanchaCommand(UpdateHorarioCanchaDto updateDto) => UpdateDto = updateDto;
        public UpdateHorarioCanchaDto UpdateDto { get; set; }
    }
}
