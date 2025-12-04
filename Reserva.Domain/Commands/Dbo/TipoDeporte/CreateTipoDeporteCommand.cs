using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.TipoDeporte;

namespace Reserva.Domain.Commands.Dbo.TipoDeporte
{
    public class CreateTipoDeporteCommand : CommandBase<GetTipoDeporteDto>
    {
        public CreateTipoDeporteCommand(CreateTipoDeporteDto createDto) => CreateDto = createDto;
        public CreateTipoDeporteDto CreateDto { get; set; }
    }
}
