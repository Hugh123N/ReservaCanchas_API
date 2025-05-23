using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Zona;

namespace Reserva.Domain.Commands.Cancha.Zona
{
    public class UpdateZonaCommand : CommandBase<GetZonaDto>
    {
        public UpdateZonaCommand(UpdateZonaDto updateDto) => UpdateDto = updateDto;
        public UpdateZonaDto UpdateDto { get; set; }
    }
}
