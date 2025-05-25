using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Disponibilidad;

namespace Reserva.Domain.Commands.Cancha.Disponibilidad
{
    public class UpdateDisponibilidadCommand : CommandBase<GetDisponibilidadDto>
    {
        public UpdateDisponibilidadCommand(UpdateDisponibilidadDto updateDto) => UpdateDto = updateDto;
        public UpdateDisponibilidadDto UpdateDto { get; set; }
    }
}
