using FluentValidation;
using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Reserva
{
    public class ConfirmarReservaOperadorCommandValidator : CommandValidatorBase<ConfirmarReservaOperadorCommand>
    {
        public ConfirmarReservaOperadorCommandValidator()
        {
            RuleFor(x => x.ConfirmarDto.IdReserva)
                .GreaterThan(0)
                .WithMessage("El ID de la reserva es requerido.");

            RuleFor(x => x.ConfirmarDto.MontoAdelanto)
                .GreaterThanOrEqualTo(0)
                .When(x => x.ConfirmarDto.MontoAdelanto.HasValue)
                .WithMessage("El monto del adelanto no puede ser negativo.");

            RuleFor(x => x.ConfirmarDto.NumeroRecibo)
                .MaximumLength(50)
                .When(x => !string.IsNullOrEmpty(x.ConfirmarDto.NumeroRecibo))
                .WithMessage("El número de recibo no puede exceder 50 caracteres.");

            RuleFor(x => x.ConfirmarDto.ObservacionOperador)
                .MaximumLength(500)
                .When(x => !string.IsNullOrEmpty(x.ConfirmarDto.ObservacionOperador))
                .WithMessage("Las observaciones no pueden exceder 500 caracteres.");
        }
    }
}
