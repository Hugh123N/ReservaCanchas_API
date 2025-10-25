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

                RuleFor(x => x.CreateDto.PrecioHora)
                    .GreaterThan(0).WithMessage("El precio por hora debe ser mayor a 0.");

                RuleFor(x => x.CreateDto.IdTipoCancha)
                    .GreaterThan(0).WithMessage("Debe seleccionar un tipo de cancha válido.");

                RuleFor(x => x.CreateDto.CodigoUbigeo)
                    .NotEmpty().WithMessage("Debe seleccionar una ubicación válida.");

                RuleFor(x => x.CreateDto.Disponibilidades)
                    .NotNull().WithMessage("Debe especificar al menos una disponibilidad.")
                    .Must(d => d.Any()).WithMessage("Debe especificar al menos una disponibilidad.")
                    .DependentRules(() =>
                    {
                        RuleForEach(x => x.CreateDto.Disponibilidades).ChildRules(d =>
                        {
                            d.RuleFor(y => y.IdDiaSemana)
                                .GreaterThan(0).WithMessage("El día de la semana es obligatorio.");

                            d.RuleFor(y => y.HoraInicio)
                                .LessThan(y => y.HoraFin)
                                .WithMessage("La hora de inicio debe ser menor que la hora de fin.");

                            d.RuleFor(y => y.HoraInicio)
                                .NotEqual(default(TimeOnly))
                                .WithMessage("Debe especificar la hora de inicio.");

                            d.RuleFor(y => y.HoraFin)
                                .NotEqual(default(TimeOnly))
                                .WithMessage("Debe especificar la hora de fin.");
                        });
                    });
            });
        }
    }
}
