using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.TipoSuperficie
{
    public class CreateTipoSuperficieCommandValidator : CommandValidatorBase<CreateTipoSuperficieCommand>
    {
        public CreateTipoSuperficieCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
