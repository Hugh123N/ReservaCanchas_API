using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Cancha;

namespace Reserva.Domain.Commands.Cancha.Cancha
{
    public class CreateCanchaCommand : CommandBase<GetCanchaDto>
    {
        public CreateCanchaCommand(CreateCanchaDto createDto) => CreateDto = createDto;
        public CreateCanchaDto CreateDto { get; set; }
    }
}
