using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.TipoCancha;

namespace Reserva.Domain.Commands.Dbo.TipoCancha
{
    public class CreateTipoCanchaCommand : CommandBase<GetTipoCanchaDto>
    {
        public CreateTipoCanchaCommand(CreateTipoCanchaDto createDto) => CreateDto = createDto;
        public CreateTipoCanchaDto CreateDto { get; set; }
    }
}
