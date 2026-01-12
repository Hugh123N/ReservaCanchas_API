using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Calendario;

namespace Reserva.Domain.Commands.Dbo.Calendario
{
    public class CrearReservaOperadorCommand : CommandBase<ReservaOperadorResponseDto>
    {
        public CrearReservaOperadorRequestDto RequestDto { get; set; } = null!;
    }
}
