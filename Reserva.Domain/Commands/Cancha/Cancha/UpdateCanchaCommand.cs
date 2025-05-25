using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Cancha;

namespace Reserva.Domain.Commands.Cancha.Cancha
{
    public class UpdateCanchaCommand : CommandBase<GetCanchaDto>
    {
        public UpdateCanchaCommand(UpdateCanchaDto updateDto) => UpdateDto = updateDto;
        public UpdateCanchaDto UpdateDto { get; set; }
    }
}
