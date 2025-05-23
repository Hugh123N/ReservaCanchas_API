using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.Departamento
{
    public class CreateDepartamentoCommandValidator : CommandValidatorBase<CreateDepartamentoCommand>
    {
        public CreateDepartamentoCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
