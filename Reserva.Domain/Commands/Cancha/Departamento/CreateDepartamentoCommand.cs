using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Departamento;

namespace Reserva.Domain.Commands.Cancha.Departamento
{
    public class CreateDepartamentoCommand : CommandBase<GetDepartamentoDto>
    {
        public CreateDepartamentoCommand(CreateDepartamentoDto createDto) => CreateDto = createDto;
        public CreateDepartamentoDto CreateDto { get; set; }
    }
}
