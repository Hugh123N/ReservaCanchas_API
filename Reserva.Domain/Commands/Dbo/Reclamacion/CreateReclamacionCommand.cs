using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.Reclamacion;

namespace Reserva.Domain.Commands.Dbo.Reclamacion
{
    public class CreateReclamacionCommand : CommandBase<GetReclamacionDto>
    {
        public CreateReclamacionCommand(CreateReclamacionDto createDto) => CreateDto = createDto;
        public CreateReclamacionDto CreateDto { get; set; }
    }
}
