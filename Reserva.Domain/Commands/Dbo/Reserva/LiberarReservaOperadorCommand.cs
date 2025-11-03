using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Reserva;

namespace Reserva.Domain.Commands.Dbo.Reserva
{
    public class LiberarReservaOperadorCommand : CommandBase<GetReservaDto>
    {
        public LiberarReservaOperadorCommand(LiberarReservaOperadorDto liberarDto)
            => LiberarDto = liberarDto;

        public LiberarReservaOperadorDto LiberarDto { get; set; }
    }
}
