using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.DiaSemana;

namespace Reserva.Domain.Commands.Dbo.DiaSemana
{
    public class UpdateDiaSemanaCommand : CommandBase<GetDiaSemanaDto>
    {
        public UpdateDiaSemanaCommand(UpdateDiaSemanaDto updateDto) => UpdateDto = updateDto;
        public UpdateDiaSemanaDto UpdateDto { get; set; }
    }
}
