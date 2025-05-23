using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Disponibilidad;

namespace Reserva.Domain.Commands.Cancha.Disponibilidad
{
    public class CreateDisponibilidadCommand : CommandBase<GetDisponibilidadDto>
    {
        public CreateDisponibilidadCommand(CreateDisponibilidadDto createDto) => CreateDto = createDto;
        public CreateDisponibilidadDto CreateDto { get; set; }
    }
}
