using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Provincia;

namespace Reserva.Domain.Commands.Cancha.Provincia
{
    public class CreateProvinciaCommand : CommandBase<GetProvinciaDto>
    {
        public CreateProvinciaCommand(CreateProvinciaDto createDto) => CreateDto = createDto;
        public CreateProvinciaDto CreateDto { get; set; }
    }
}
