using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.TipoCancha;

namespace Reserva.Domain.Commands.Dbo.TipoCancha
{
    public class UpdateTipoCanchaCommand : CommandBase<GetTipoCanchaDto>
    {
        public UpdateTipoCanchaCommand(UpdateTipoCanchaDto updateDto) => UpdateDto = updateDto;
        public UpdateTipoCanchaDto UpdateDto { get; set; }
    }
}
