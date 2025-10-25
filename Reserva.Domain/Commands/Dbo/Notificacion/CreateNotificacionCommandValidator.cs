using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Notificacion
{
    public class CreateNotificacionCommandValidator : CommandValidatorBase<CreateNotificacionCommand>
    {
        public CreateNotificacionCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
