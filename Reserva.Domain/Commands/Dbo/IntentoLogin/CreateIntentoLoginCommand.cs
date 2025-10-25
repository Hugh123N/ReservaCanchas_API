using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.IntentoLogin;

namespace Reserva.Domain.Commands.Dbo.IntentoLogin
{
    public class CreateIntentoLoginCommand : CommandBase<GetIntentoLoginDto>
    {
        public CreateIntentoLoginCommand(CreateIntentoLoginDto createDto) => CreateDto = createDto;
        public CreateIntentoLoginDto CreateDto { get; set; }
    }
}
