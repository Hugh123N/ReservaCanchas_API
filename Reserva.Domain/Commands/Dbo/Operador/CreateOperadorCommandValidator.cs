using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Operador
{
    public class CreateOperadorCommandValidator : CommandValidatorBase<CreateOperadorCommand>
    {
        public CreateOperadorCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
