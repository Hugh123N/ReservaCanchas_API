using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.TipoSuperficie;

namespace Reserva.Domain.Commands.Dbo.TipoSuperficie
{
    public class CreateTipoSuperficieCommand : CommandBase<GetTipoSuperficieDto>
    {
        public CreateTipoSuperficieCommand(CreateTipoSuperficieDto createDto) => CreateDto = createDto;
        public CreateTipoSuperficieDto CreateDto { get; set; }
    }
}
