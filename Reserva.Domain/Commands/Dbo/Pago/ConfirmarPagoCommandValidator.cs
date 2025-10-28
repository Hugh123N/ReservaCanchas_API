using FluentValidation;
using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Pago
{
    public class ConfirmarPagoCommandValidator : CommandValidatorBase<ConfirmarPagoCommand>
    {
        public ConfirmarPagoCommandValidator()
        {
            RequiredInformation(x => x.ConfirmarDto).DependentRules(() =>
            {
                // Validar IdPago
                RuleFor(x => x.ConfirmarDto.IdPago)
                    .GreaterThan(0)
                    .WithMessage("El ID del pago es obligatorio y debe ser mayor a 0.");

                // Validar CodigoOperacion
                RuleFor(x => x.ConfirmarDto.CodigoOperacion)
                    .NotEmpty()
                    .WithMessage("El código de operación es obligatorio.")
                    .Length(6, 10)
                    .WithMessage("El código de operación debe tener entre 6 y 10 caracteres.")
                    .Matches(@"^[A-Z0-9]+$")
                    .WithMessage("El código de operación solo debe contener letras mayúsculas y números.");
            });
        }
    }
}
