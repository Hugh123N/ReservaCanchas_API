using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Reserva;

namespace Reserva.Domain.Commands.Dbo.Reserva
{
    public class ConfirmarReservaOperadorCommand : CommandBase<GetReservaDto>
    {
        public ConfirmarReservaOperadorCommand(ConfirmarReservaOperadorDto confirmarDto)
            => ConfirmarDto = confirmarDto;

        public ConfirmarReservaOperadorDto ConfirmarDto { get; set; }
    }
}
