using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.IntentoLogin;

namespace Reserva.Domain.Commands.Cancha.IntentoLogin
{
    public class UpdateIntentoLoginCommand : CommandBase<GetIntentoLoginDto>
    {
        public UpdateIntentoLoginCommand(UpdateIntentoLoginDto updateDto) => UpdateDto = updateDto;
        public UpdateIntentoLoginDto UpdateDto { get; set; }
    }
}
