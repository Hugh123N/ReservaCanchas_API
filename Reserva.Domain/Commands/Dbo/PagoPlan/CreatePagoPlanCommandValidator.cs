using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.PagoPlan
{
    public class CreatePagoPlanCommandValidator : CommandValidatorBase<CreatePagoPlanCommand>
    {
        public CreatePagoPlanCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
