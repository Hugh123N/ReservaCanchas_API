using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.DiaSemana;

namespace Reserva.Domain.Commands.Cancha.DiaSemana
{
    public class CreateDiaSemanaCommand : CommandBase<GetDiaSemanaDto>
    {
        public CreateDiaSemanaCommand(CreateDiaSemanaDto createDto) => CreateDto = createDto;
        public CreateDiaSemanaDto CreateDto { get; set; }
    }
}
