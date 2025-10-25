using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Cancha;

namespace Reserva.Domain.Commands.Dbo.Cancha
{
    public class UpdateCanchaCommand : CommandBase<GetCanchaDto>
    {
        public UpdateCanchaCommand(UpdateCanchaDto updateDto) => UpdateDto = updateDto;
        public UpdateCanchaDto UpdateDto { get; set; }
    }
}
