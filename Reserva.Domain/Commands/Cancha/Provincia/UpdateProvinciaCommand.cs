using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Provincia;

namespace Reserva.Domain.Commands.Cancha.Provincia
{
    public class UpdateProvinciaCommand : CommandBase<GetProvinciaDto>
    {
        public UpdateProvinciaCommand(UpdateProvinciaDto updateDto) => UpdateDto = updateDto;
        public UpdateProvinciaDto UpdateDto { get; set; }
    }
}
