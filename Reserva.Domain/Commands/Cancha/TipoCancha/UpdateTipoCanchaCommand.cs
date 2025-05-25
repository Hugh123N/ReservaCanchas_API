using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.TipoCancha;

namespace Reserva.Domain.Commands.Cancha.TipoCancha
{
    public class UpdateTipoCanchaCommand : CommandBase<GetTipoCanchaDto>
    {
        public UpdateTipoCanchaCommand(UpdateTipoCanchaDto updateDto) => UpdateDto = updateDto;
        public UpdateTipoCanchaDto UpdateDto { get; set; }
    }
}
