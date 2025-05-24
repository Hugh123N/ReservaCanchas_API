using Reserva.Domain.Commands.Base;
using Reserva.Dto.Cancha.Ubigeo;

namespace Reserva.Domain.Commands.Cancha.Ubigeo
{
    public class UpdateUbigeoCommand : CommandBase<GetUbigeoDto>
    {
        public UpdateUbigeoCommand(UpdateUbigeoDto updateDto) => UpdateDto = updateDto;
        public UpdateUbigeoDto UpdateDto { get; set; }
    }
}
