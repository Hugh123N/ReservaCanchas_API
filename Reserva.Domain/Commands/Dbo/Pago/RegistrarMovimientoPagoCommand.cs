using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Pago;

namespace Reserva.Domain.Commands.Dbo.Pago;

public class RegistrarMovimientoPagoCommand(RegistrarMovimientoPagoDto dto, bool correccion) : CommandBase<GetHistorialPagoDto>
{
    public RegistrarMovimientoPagoDto Dto { get; } = dto;
    public bool Correccion { get; } = correccion;
}
