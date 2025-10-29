using FluentValidation;
using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Pago
{
    public class CompletarPagoCommandValidator : CommandValidatorBase<CompletarPagoCommand>
    {
        public CompletarPagoCommandValidator()
        {
            RequiredInformation(x => x.CompletarDto).DependentRules(() =>
            {
                // Validar IdPago
                RuleFor(x => x.CompletarDto.IdPago)
                    .GreaterThan(0)
                    .WithMessage("El ID del pago es obligatorio y debe ser mayor a 0.");

                // Validar MontoRestante
                RuleFor(x => x.CompletarDto.MontoRestante)
                    .GreaterThan(0)
                    .WithMessage("El monto restante debe ser mayor a 0.");
            });
        }
    }
}
