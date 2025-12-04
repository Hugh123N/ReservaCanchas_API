using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.TipoDeporte;

namespace Reserva.Domain.Commands.Dbo.TipoDeporte
{
    public class UpdateTipoDeporteCommand : CommandBase<GetTipoDeporteDto>
    {
        public UpdateTipoDeporteCommand(UpdateTipoDeporteDto updateDto) => UpdateDto = updateDto;
        public UpdateTipoDeporteDto UpdateDto { get; set; }
    }
}
