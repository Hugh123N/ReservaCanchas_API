using FluentValidation;
using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Cancha
{
    public class CreateCanchaCommandValidator : CommandValidatorBase<CreateCanchaCommand>
    {
        public CreateCanchaCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                RuleFor(x => x.CreateDto.Nombre)
                    .NotEmpty().WithMessage("El nombre de la cancha es obligatorio.")
                    .MaximumLength(100).WithMessage("El nombre de la cancha no puede superar los 100 caracteres.");

                RuleFor(x => x.CreateDto.Descripcion)
                    .NotEmpty().WithMessage("La descripción de la cancha es obligatoria.")
                    .MaximumLength(500).WithMessage("La descripción no puede superar los 500 caracteres.");

                RuleFor(x => x.CreateDto.Precio)
                    .GreaterThan(0).WithMessage("El precio por hora debe ser mayor a 0.");

                RuleFor(x => x.CreateDto.IdTipoSuperficie)
                    .GreaterThan(0).WithMessage("Debe seleccionar un tipo de superficie válido.");

                RuleFor(x => x.CreateDto.HorarioCanchas)
                    .NotNull().WithMessage("Debe especificar al menos un Horario Cancha.");
            });
        }
    }
}
