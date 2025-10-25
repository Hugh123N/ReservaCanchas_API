using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Cancha;

namespace Reserva.Domain.Commands.Dbo.Cancha
{
    public class CreateCanchaCommand : CommandBase<GetCanchaDto>
    {
        public CreateCanchaCommand(CreateCanchaDto createDto) => CreateDto = createDto;
        public CreateCanchaDto CreateDto { get; set; }
    }
}
