using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.PagoPlan;

namespace Reserva.Domain.Commands.Dbo.PagoPlan
{
    public class UpdatePagoPlanCommand : CommandBase<GetPagoPlanDto>
    {
        public UpdatePagoPlanCommand(UpdatePagoPlanDto updateDto) => UpdateDto = updateDto;
        public UpdatePagoPlanDto UpdateDto { get; set; }
    }
}
