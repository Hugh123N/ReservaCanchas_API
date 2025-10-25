using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Ubigeo
{
    public class CreateUbigeoCommandValidator : CommandValidatorBase<CreateUbigeoCommand>
    {
        public CreateUbigeoCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
