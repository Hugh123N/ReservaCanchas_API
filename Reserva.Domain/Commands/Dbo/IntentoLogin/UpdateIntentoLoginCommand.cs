using Reserva.Domain.Commands.Base;
using Reserva.Dto.Dbo.IntentoLogin;

namespace Reserva.Domain.Commands.Dbo.IntentoLogin
{
    public class UpdateIntentoLoginCommand : CommandBase<GetIntentoLoginDto>
    {
        public UpdateIntentoLoginCommand(UpdateIntentoLoginDto updateDto) => UpdateDto = updateDto;
        public UpdateIntentoLoginDto UpdateDto { get; set; }
    }
}
