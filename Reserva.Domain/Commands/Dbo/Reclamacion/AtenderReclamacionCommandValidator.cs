using FluentValidation;
using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Reclamacion
{
    public class AtenderReclamacionCommandValidator : CommandValidatorBase<AtenderReclamacionCommand>
    {
        public AtenderReclamacionCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RequiredInformation(x => x.AtenderDto).DependentRules(() =>
            {
                RuleFor(x => x.AtenderDto.Estado).Must(x => x is "EN_ATENCION" or "RESPONDIDA");
                RuleFor(x => x.AtenderDto.Observaciones).MaximumLength(2000);
                RuleFor(x => x.AtenderDto.Respuesta).MaximumLength(4000);
                RuleFor(x => x.AtenderDto.ReferenciaRespuesta).MaximumLength(250);
                When(x => x.AtenderDto.Estado == "RESPONDIDA", () =>
                {
                    RuleFor(x => x.AtenderDto.Respuesta).NotEmpty().WithMessage("Registra la respuesta enviada al consumidor.");
                    RuleFor(x => x.AtenderDto.ReferenciaRespuesta).NotEmpty().WithMessage("Indica la referencia del correo o carta enviada.");
                    RuleFor(x => x.AtenderDto.FechaComunicacionRespuesta).NotNull()
                        .Must(x => x <= DateTimeOffset.UtcNow).WithMessage("La fecha de respuesta no puede ser futura.");
                });
            });
        }
    }
}
