using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Operador;

namespace Reserva.Domain.Commands.Dbo.Operador
{
    public class UpdateOperadorCommand : CommandBase<GetOperadorDto>
    {
        public UpdateOperadorCommand(UpdateOperadorDto updateDto) => UpdateDto = updateDto;
        public UpdateOperadorDto UpdateDto { get; set; }
    }
}
