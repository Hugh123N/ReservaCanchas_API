using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Cancha.Ubigeo
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
