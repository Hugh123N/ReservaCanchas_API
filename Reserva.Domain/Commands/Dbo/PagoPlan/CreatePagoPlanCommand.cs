using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.PagoPlan;

namespace Reserva.Domain.Commands.Dbo.PagoPlan
{
    public class CreatePagoPlanCommand : CommandBase<GetPagoPlanDto>
    {
        public CreatePagoPlanCommand(CreatePagoPlanDto createDto) => CreateDto = createDto;
        public CreatePagoPlanDto CreateDto { get; set; }
    }
}
