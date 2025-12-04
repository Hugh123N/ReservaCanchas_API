using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Servicio
{
    public class CreateServicioCommandValidator : CommandValidatorBase<CreateServicioCommand>
    {
        public CreateServicioCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
