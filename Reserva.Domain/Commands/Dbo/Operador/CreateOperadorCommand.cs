using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Operador;

namespace Reserva.Domain.Commands.Dbo.Operador
{
    public class CreateOperadorCommand : CommandBase<GetOperadorDto>
    {
        public CreateOperadorCommand(CreateOperadorDto createDto) => CreateDto = createDto;
        public CreateOperadorDto CreateDto { get; set; }
    }
}
