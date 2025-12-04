using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Servicio;

namespace Reserva.Domain.Commands.Dbo.Servicio
{
    public class CreateServicioCommand : CommandBase<GetServicioDto>
    {
        public CreateServicioCommand(CreateServicioDto createDto) => CreateDto = createDto;
        public CreateServicioDto CreateDto { get; set; }
    }
}
