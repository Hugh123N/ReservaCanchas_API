using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Cliente
{
    public class CreateClienteCommandValidator : CommandValidatorBase<CreateClienteCommand>
    {
        public CreateClienteCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
