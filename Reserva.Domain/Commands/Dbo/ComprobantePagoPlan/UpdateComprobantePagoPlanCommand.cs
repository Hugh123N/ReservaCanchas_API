using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.ComprobantePagoPlan;

namespace Reserva.Domain.Commands.Dbo.ComprobantePagoPlan
{
    public class UpdateComprobantePagoPlanCommand : CommandBase<GetComprobantePagoPlanDto>
    {
        public UpdateComprobantePagoPlanCommand(UpdateComprobantePagoPlanDto updateDto) => UpdateDto = updateDto;
        public UpdateComprobantePagoPlanDto UpdateDto { get; set; }
    }
}
