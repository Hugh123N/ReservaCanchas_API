using Reserva.Domain.Commands.Base;

namespace Reserva.Domain.Commands.Dbo.DetalleReserva
{
    public class CreateDetalleReservaCommandValidator : CommandValidatorBase<CreateDetalleReservaCommand>
    {
        public CreateDetalleReservaCommandValidator()
        {
            RequiredInformation(x => x.CreateDto).DependentRules(() =>
            {
                
            });
        }
    }
}
