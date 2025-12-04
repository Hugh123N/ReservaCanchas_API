using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Servicio;

namespace Reserva.Domain.Commands.Dbo.Servicio
{
    public class UpdateServicioCommand : CommandBase<GetServicioDto>
    {
        public UpdateServicioCommand(UpdateServicioDto updateDto) => UpdateDto = updateDto;
        public UpdateServicioDto UpdateDto { get; set; }
    }
}
