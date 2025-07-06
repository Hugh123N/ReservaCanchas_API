using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.IntentoLogin;

namespace Reserva.Domain.Commands.Cancha.IntentoLogin
{
    public class CreateIntentoLoginCommand : CommandBase<GetIntentoLoginDto>
    {
        public CreateIntentoLoginCommand(CreateIntentoLoginDto createDto) => CreateDto = createDto;
        public CreateIntentoLoginDto CreateDto { get; set; }
    }
}
