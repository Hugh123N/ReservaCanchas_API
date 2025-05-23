using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Departamento;

namespace Reserva.Domain.Commands.Cancha.Departamento
{
    public class UpdateDepartamentoCommand : CommandBase<GetDepartamentoDto>
    {
        public UpdateDepartamentoCommand(UpdateDepartamentoDto updateDto) => UpdateDto = updateDto;
        public UpdateDepartamentoDto UpdateDto { get; set; }
    }
}
