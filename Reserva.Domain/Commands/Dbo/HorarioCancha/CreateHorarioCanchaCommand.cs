using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.HorarioCancha;

namespace Reserva.Domain.Commands.Dbo.HorarioCancha
{
    public class CreateHorarioCanchaCommand : CommandBase<GetHorarioCanchaDto>
    {
        public CreateHorarioCanchaCommand(CreateHorarioCanchaDto createDto) => CreateDto = createDto;
        public CreateHorarioCanchaDto CreateDto { get; set; }
    }
}
