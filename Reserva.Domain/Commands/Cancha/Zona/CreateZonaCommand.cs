using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Zona;

namespace Reserva.Domain.Commands.Cancha.Zona
{
    public class CreateZonaCommand : CommandBase<GetZonaDto>
    {
        public CreateZonaCommand(CreateZonaDto createDto) => CreateDto = createDto;
        public CreateZonaDto CreateDto { get; set; }
    }
}
