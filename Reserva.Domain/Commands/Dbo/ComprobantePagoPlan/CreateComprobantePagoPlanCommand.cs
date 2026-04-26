using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.ComprobantePagoPlan;

namespace Reserva.Domain.Commands.Dbo.ComprobantePagoPlan
{
    public class CreateComprobantePagoPlanCommand : CommandBase<GetComprobantePagoPlanDto>
    {
        public CreateComprobantePagoPlanCommand(CreateComprobantePagoPlanDto createDto) => CreateDto = createDto;
        public CreateComprobantePagoPlanDto CreateDto { get; set; }
    }
}
