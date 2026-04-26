using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.Plane
{
    public class CreatePlaneCommandValidator : CommandValidatorBase<CreatePlaneCommand>
    {
        public CreatePlaneCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
