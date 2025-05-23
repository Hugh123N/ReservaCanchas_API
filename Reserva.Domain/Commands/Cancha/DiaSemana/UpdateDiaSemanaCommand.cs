using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.DiaSemana;

namespace Reserva.Domain.Commands.Cancha.DiaSemana
{
    public class UpdateDiaSemanaCommand : CommandBase<GetDiaSemanaDto>
    {
        public UpdateDiaSemanaCommand(UpdateDiaSemanaDto updateDto) => UpdateDto = updateDto;
        public UpdateDiaSemanaDto UpdateDto { get; set; }
    }
}
