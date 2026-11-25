using FluentValidation;
using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Reclamacion
{
    public class CreateReclamacionCommandValidator : CommandValidatorBase<CreateReclamacionCommand>
    {
        public CreateReclamacionCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                RuleFor(x => x.CreateDto.Nombres).NotEmpty().MaximumLength(200);
                RuleFor(x => x.CreateDto.TipDoc).Must(x => x is "1" or "4" or "7").WithMessage("Selecciona un tipo de documento válido.");
                RuleFor(x => x.CreateDto.NroDoc).NotEmpty().MaximumLength(20)
                    .Matches("^[a-zA-Z0-9]+$").WithMessage("El documento solo admite letras y números.");
                RuleFor(x => x.CreateDto.NroDoc).Matches("^[0-9]{8}$").When(x => x.CreateDto.TipDoc == "1").WithMessage("El DNI debe tener 8 dígitos.");
                RuleFor(x => x.CreateDto.Domicilio).NotEmpty().MaximumLength(400);
                RuleFor(x => x.CreateDto.Email).NotEmpty().EmailAddress().MaximumLength(254);
                RuleFor(x => x.CreateDto.Telefono).MaximumLength(20).Matches(@"^\+?[0-9 ()-]{6,20}$")
                    .When(x => !string.IsNullOrEmpty(x.CreateDto.Telefono));
                RuleFor(x => x.CreateDto.Representante).MaximumLength(200);
                RuleFor(x => x.CreateDto.Representante).NotEmpty().When(x => x.CreateDto.EsMenor)
                    .WithMessage("Indica el nombre del padre, madre o representante.");
                RuleFor(x => x.CreateDto.DescripcionServicio).NotEmpty().MaximumLength(500);
                RuleFor(x => x.CreateDto.MontoReclamado).InclusiveBetween(0m, 9999999999.99m).PrecisionScale(12, 2, true);
                RuleFor(x => x.CreateDto.ReferenciaOperacion).MaximumLength(100);
                RuleFor(x => x.CreateDto.Tipo).Must(x => x is "RECLAMO" or "QUEJA").WithMessage("Selecciona reclamo o queja.");
                RuleFor(x => x.CreateDto.Detalle).NotEmpty().MaximumLength(4000);
                RuleFor(x => x.CreateDto.Pedido).NotEmpty().MaximumLength(2000);
            });
        }
    }
}
