using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Disponibilidad;

namespace Reserva.Domain.Commands.Dbo.Disponibilidad
{
    public class CreateDisponibilidadCommand : CommandBase<GetDisponibilidadDto>
    {
        public CreateDisponibilidadCommand(CreateDisponibilidadDto createDto) => CreateDto = createDto;
        public CreateDisponibilidadDto CreateDto { get; set; }
    }
}
