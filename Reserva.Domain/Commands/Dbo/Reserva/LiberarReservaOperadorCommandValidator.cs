using FluentValidation;
using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Reserva
{
    public class LiberarReservaOperadorCommandValidator : CommandValidatorBase<LiberarReservaOperadorCommand>
    {
        public LiberarReservaOperadorCommandValidator()
        {
            RuleFor(x => x.LiberarDto.IdReserva)
                .GreaterThan(0)
                .WithMessage("El ID de la reserva es requerido.");

            RuleFor(x => x.LiberarDto.MotivoLiberacion)
                .MaximumLength(500)
                .When(x => !string.IsNullOrEmpty(x.LiberarDto.MotivoLiberacion))
                .WithMessage("El motivo no puede exceder 500 caracteres.");

            RuleFor(x => x.LiberarDto.MontoReembolso)
                .GreaterThanOrEqualTo(0)
                .WithMessage("El monto de reembolso no puede ser negativo.");
        }
    }
}
