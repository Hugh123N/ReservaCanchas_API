using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.TipoDeporte
{
    public class CreateTipoDeporteCommandValidator : CommandValidatorBase<CreateTipoDeporteCommand>
    {
        public CreateTipoDeporteCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
