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
            RuleFor(x => x.LiberarDto.MontoReembolso).Must(m => decimal.Round(m, 2) == m);
            RuleFor(x => x.LiberarDto.ReferenciaOperacion).MaximumLength(100);
            When(x => x.LiberarDto.MontoReembolso > 0, () =>
            {
                RuleFor(x => x.LiberarDto.IdOperacion).NotEmpty();
                RuleFor(x => x.LiberarDto.RevisionPago).NotNull().GreaterThanOrEqualTo(0);
                RuleFor(x => x.LiberarDto.CodigoMetodoPago)
                    .Must(c => c == "02" || c == "03" || c == "04" || c == "05")
                    .WithMessage("Selecciona el método utilizado para devolver el dinero.");
            });
        }
    }
}
