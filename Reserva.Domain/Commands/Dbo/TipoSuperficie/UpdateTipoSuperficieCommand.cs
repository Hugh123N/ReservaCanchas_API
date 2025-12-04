using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.TipoSuperficie;

namespace Reserva.Domain.Commands.Dbo.TipoSuperficie
{
    public class UpdateTipoSuperficieCommand : CommandBase<GetTipoSuperficieDto>
    {
        public UpdateTipoSuperficieCommand(UpdateTipoSuperficieDto updateDto) => UpdateDto = updateDto;
        public UpdateTipoSuperficieDto UpdateDto { get; set; }
    }
}
