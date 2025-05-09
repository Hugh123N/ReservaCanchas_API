using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.DiaSemana;

namespace Reserva.Domain.Commands.Dbo.DiaSemana
{
    public class CreateDiaSemanaCommand : CommandBase<GetDiaSemanaDto>
    {
        public CreateDiaSemanaCommand(CreateDiaSemanaDto createDto) => CreateDto = createDto;
        public CreateDiaSemanaDto CreateDto { get; set; }
    }
}
