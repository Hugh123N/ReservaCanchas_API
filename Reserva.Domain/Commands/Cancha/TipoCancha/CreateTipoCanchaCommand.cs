using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.TipoCancha;

namespace Reserva.Domain.Commands.Cancha.TipoCancha
{
    public class CreateTipoCanchaCommand : CommandBase<GetTipoCanchaDto>
    {
        public CreateTipoCanchaCommand(CreateTipoCanchaDto createDto) => CreateDto = createDto;
        public CreateTipoCanchaDto CreateDto { get; set; }
    }
}
