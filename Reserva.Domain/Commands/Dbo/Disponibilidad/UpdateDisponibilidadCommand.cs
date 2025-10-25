using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Disponibilidad;

namespace Reserva.Domain.Commands.Dbo.Disponibilidad
{
    public class UpdateDisponibilidadCommand : CommandBase<GetDisponibilidadDto>
    {
        public UpdateDisponibilidadCommand(UpdateDisponibilidadDto updateDto) => UpdateDto = updateDto;
        public UpdateDisponibilidadDto UpdateDto { get; set; }
    }
}
