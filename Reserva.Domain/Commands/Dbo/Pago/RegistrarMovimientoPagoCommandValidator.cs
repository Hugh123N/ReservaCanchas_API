using FluentValidation;
using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Pago;

public class RegistrarMovimientoPagoCommandValidator : CommandValidatorBase<RegistrarMovimientoPagoCommand>
{
    public RegistrarMovimientoPagoCommandValidator()
    {
        RuleFor(x => x.Dto.IdPago).GreaterThanOrEqualTo(0);
        When(x => x.Dto.IdReserva.HasValue, () => RuleFor(x => x.Dto.IdReserva).GreaterThan(0));
        RuleFor(x => x.Dto).Must(d => d.IdPago > 0 || d.IdReserva > 0)
            .WithMessage("Indica el pago o la reserva del primer cobro.");
        When(x => x.Correccion, () => RuleFor(x => x.Dto.IdPago).GreaterThan(0));
        RuleFor(x => x.Dto.IdOperacion).NotEmpty();
        RuleFor(x => x.Dto.Revision).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Dto.Monto).InclusiveBetween(0, 99999999.99m)
            .Must(m => decimal.Round(m, 2) == m).WithMessage("El importe debe tener como máximo dos decimales.");
        When(x => !x.Correccion, () => RuleFor(x => x.Dto.Monto).GreaterThan(0));
        RuleFor(x => x.Dto.Motivo).MaximumLength(500);
        RuleFor(x => x.Dto.ReferenciaOperacion).MaximumLength(100);
        When(x => x.Correccion, () => RuleFor(x => x.Dto.Motivo).Must(m => m?.Trim().Length >= 5)
            .WithMessage("Indica un motivo de corrección de al menos 5 caracteres."));
    }
}
